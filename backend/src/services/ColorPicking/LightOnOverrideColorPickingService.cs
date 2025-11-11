using System;

namespace AlarmClock.Backend.Services
{
    public class LightOnOverrideColorPickingService : IOverrideColorPickingService
    {
        public string guid { get; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        private readonly AlarmClockColor _onColor;

        public LightOnOverrideColorPickingService(DateTime endTime, AlarmClockColor onColor = null)
        {
            guid = Guid.NewGuid().ToString();
            StartTime = DateTime.Now;
            EndTime = endTime;

            // Use a warm white color for the override if none is provided
            _onColor = onColor ?? new AlarmClockColor(
                new LifxNet.Color { R = 255, G = 255, B = 255 },
                3500 // Warm white kelvin
            );
        }

        public AlarmClockColor GetColorForTime(DateTime time)
        {
            // Return the on color if within the active time range
            if (IsLightOnAtTime(time))
            {
                return _onColor;
            }

            // Return default (off) color if outside the time range
            return AlarmClockColor.Default;
        }

        public bool IsLightOnAtTime(DateTime time)
        {
            // Light should be on if time is between StartTime and EndTime
            return time >= StartTime && time <= EndTime;
        }

        public string Status(DateTime time)
        {
            if (IsLightOnAtTime(time))
            {
                return $"On until {EndTime:h:mm tt}";
            }

            return "Override inactive";
        }
    }
}
