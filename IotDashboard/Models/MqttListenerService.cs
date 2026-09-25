using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging; 
using MQTTnet;
using System.Text;
using System.Text.Json;

namespace IotDashboard.Models
{
    public class MqttListenerService : BackgroundService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private readonly ILogger<MqttListenerService> _logger;

       
        public MqttListenerService(ILogger<MqttListenerService> logger)
        {
            _logger = logger; 

            var mqttFactory = new MqttClientFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .WithClientId("CSharp_IotDashboard_Client")
                .WithCleanSession()
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

             
                _logger.LogInformation($"[MQTT Verisi Geldi] Ham Payload: {payload}");

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var incomingData = JsonSerializer.Deserialize<IotData>(payload, options);

                    if (incomingData != null)
                    {
                        // IotDataStore.CurrentData.Temperature = incomingData.Temperature;
                        //IotDataStore.CurrentData.Humidity = incomingData.Humidity;
                        // IotDataStore.CurrentData.Timestamp = DateTime.Now;
                        incomingData.DeviceCode = e.ApplicationMessage.Topic switch
                        {
                            "iot/esp_emre/telemetry" => "esp_emre",
                            "iot/esp_nursemin/telemetry" => "esp_nursemin",
                            _ => incomingData.DeviceCode
                        };

                        incomingData.Timestamp = DateTime.Now;
                        IotDataStore.Add(incomingData);

                        
                        _logger.LogInformation($"[Veri İşlendi] Sıcaklık: {incomingData.Temperature}°C, Nem: {incomingData.Humidity}%");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[Veri Çözme Hatası] {ex.Message}");
                }

                return Task.CompletedTask;
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_mqttClient.IsConnected)
                    {
                        _logger.LogWarning("[MQTT] Mosquitto'ya bağlanmaya çalışılıyor...");
                        await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
                        _logger.LogInformation("[MQTT] Mosquitto brokerına başarıyla BAĞLANDI!");

                        await _mqttClient.SubscribeAsync("iot/esp_emre/telemetry", cancellationToken: stoppingToken);

                        await _mqttClient.SubscribeAsync("iot/esp_nursemin/telemetry",cancellationToken: stoppingToken);
                        _logger.LogInformation("[MQTT] İki ESP'nin telemetri konuları dinleniyor.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[Mosquitto Bağlantı Hatası] {ex.Message}. 5 saniye sonra tekrar denenecek...");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}