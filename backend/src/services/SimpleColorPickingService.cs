using System;
using LifxNet;

namespace MyFullstackApp.Services
{
    public class SimpleColorPickingService : IColorPickingService
    {
        public bool IsLightOnAtTime(DateTime time)
        {
            int minutesSinceMidnight = GetMinutesSinceMidnight(time);
            return minutesSinceMidnight >= SixAMInMinutes && minutesSinceMidnight <= EightAMInMinutes;
        }

        public AlarmClockColor GetColorForTime(DateTime time)
        {
            int minutesSinceMidnight = GetMinutesSinceMidnight(time);

            if (minutesSinceMidnight < SixAMInMinutes)
            {
                // Before 6 AM: Off
                return new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, 1500);
            }
            else if (minutesSinceMidnight >= SixAMInMinutes && minutesSinceMidnight <= EightAMInMinutes)
            {
                // Between 6 AM and 8 AM: Gradually increase brightness and color temperature
                int totalMinutes = EightAMInMinutes - SixAMInMinutes;
                int elapsedMinutes = minutesSinceMidnight - SixAMInMinutes;
                int percentage = (int)((elapsedMinutes / (double)totalMinutes) * 100);

                return ColorFromLightPercentage(percentage);
            }
            else
            {
                // After 8 AM: Full brightness and cool white
                return new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, 6500);
            }
        }



        private static int GetMinutesSinceMidnight(DateTime time)
        {
            return time.Hour * 60 + time.Minute;
        }

        private static int SixAMInMinutes = 6 * 60;
        private static int EightAMInMinutes = 8 * 60;


        private static AlarmClockColor ColorFromLightPercentage(int percentage)
        {
            // Map percentage (0-100) to a color and kelvin value
            byte brightness = BrightnessFromPercentage(percentage);  // Scale 0-100 to 0-255
            ushort kelvin = (ushort)(1500 + (percentage * 75)); // Scale to 1500-9000K

            return new AlarmClockColor(new LifxNet.Color { R = brightness, G = brightness, B = brightness }, kelvin);
        }

        private static byte BrightnessFromPercentage(int percentage)
        {
            if (percentage < 0) return (byte)0x0;
            if (percentage > 50) return (byte)0xFF;
            return (byte)(percentage * 2);
        }

        private static ushort KelvinFromPercentage(int percentage)
        {
            if (percentage < 50) return 1500;
            if (percentage > 100) return 6500;
            const ushort range = 6500 - 1500;
            double multiplier = 2 * (percentage - 50) / 100.0;
            ushort addition = (ushort)(multiplier * range);
            return (ushort)(1500 + addition);
        }
    }
}