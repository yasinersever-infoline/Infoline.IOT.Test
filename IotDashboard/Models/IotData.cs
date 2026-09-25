namespace IotDashboard.Models
{
    public class IotData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public static class IotDataStore
    { 
        // BAŞLANGIÇTA BU VERİLERLE BAŞLADIĞIMIZI VARSAYALIM.
        public static IotData CurrentData { get; set; } = new IotData
        {
            Temperature = 0.0,
            Humidity = 0.0,
            Timestamp = DateTime.UtcNow
        };
    }
}
