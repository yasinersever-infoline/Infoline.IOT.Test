using System;
using System.Collections.Generic;

namespace IotDashboard.Models
{
    public class IotData
    {
        public string? DeviceCode { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public bool? Dark { get; set; }
        public bool? Infrared { get; set; }
        public bool? LightOn { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public static class IotDataStore
    {
        private static readonly object _gate = new();
        private static readonly List<IotData> _history = new();

        public static IotData CurrentData { get; private set; } = new IotData();

        public static void Add(IotData data)
        {
            lock (_gate)
            {
                _history.Insert(0, data);
                CurrentData = data;
            }
        }

        public static IotData[] GetHistory()
        {
            lock (_gate)
            {
                return _history.ToArray();
            }
        }
    }
}