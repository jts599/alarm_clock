using System;
using LifxNet;

namespace MyFullstackApp.Services
{
    public class SimpleColorPickingService : IColorPickingService
    {
        private const int MaxKelvin = 4500;
        private const int MinKelvin = 1500;
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
                return new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, MinKelvin);
            }
            else if (minutesSinceMidnight >= SixAMInMinutes && minutesSinceMidnight <= EightAMInMinutes)
            {
                // Between 6 AM and 8 AM: Gradually increase brightness and color temperature
                int totalMinutes = EightAMInMinutes - SixAMInMinutes;
                int elapsedMinutes = minutesSinceMidnight - SixAMInMinutes;
                int percentage = (int)((elapsedMinutes / (double)totalMinutes) * 100);

                return ColorFromLightPercentage(percentage);
            }
            else if (minutesSinceMidnight > EightAMInMinutes && minutesSinceMidnight <= NineAMInMinutes)
            {
                // Between 8 AM and 9 AM: Steady bright white light
                return new AlarmClockColor(new LifxNet.Color { R = 0xFF, G = 0xFF, B = 0xFF }, MaxKelvin);
            }
            else
            {
                // After 9 AM: Off
                return new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, MaxKelvin);
            }
        }



        private static int GetMinutesSinceMidnight(DateTime time)
        {
            return time.Hour * 60 + time.Minute;
        }

        private static int SixAMInMinutes = 6 * 60;
        private static int EightAMInMinutes = 8 * 60;
        private static int NineAMInMinutes = 9 * 60;


        private static AlarmClockColor ColorFromLightPercentage(int percentage)
        {
            // Map percentage (0-100) to a color and kelvin value
            byte brightness = BrightnessFromPercentage(percentage);  // Scale 0-100 to 0-255
            ushort kelvin = KelvinFromPercentage(percentage);      // Scale 0-100 to 1500-6500K

            return new AlarmClockColor(new LifxNet.Color { R = brightness, G = brightness, B = brightness }, kelvin);
        }

        private static byte BrightnessFromPercentage(int percentage)
        {
            if (percentage < 0) return (byte)0x0;
            if (percentage > 50) return (byte)0xFF;
            return (byte)(percentage / 100.0 * 2 * 0xFF);
        }

        private static ushort KelvinFromPercentage(int percentage)
        {
            if (percentage < 50) return MinKelvin;
            if (percentage > 100) return MaxKelvin;
            const ushort range = MaxKelvin - MinKelvin;
            double multiplier = 2 * (percentage - 50) / 100.0;
            ushort addition = (ushort)(multiplier * range);
            return (ushort)(MinKelvin + addition);
        }
    }
}