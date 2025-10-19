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
            var color = GetColorForTime(time);
            return !(color.Color.R == 0 && color.Color.G == 0 && color.Color.B == 0);
        }

        public AlarmClockColor GetColorForTime(DateTime time)
        {
            int secondsSinceMidnight = GetSecondsSinceMidnight(time);

            if (secondsSinceMidnight < SixThirtyAMInSeconds)
            {
                // Before 6 AM: Off
                return new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, MinKelvin);
            }
            else if (secondsSinceMidnight >= SixThirtyAMInSeconds && secondsSinceMidnight <= EightAMInSeconds)
            {
                // Between 6 AM and 8 AM: Gradually increase brightness and color temperature
                int totalSeconds = EightAMInSeconds - SixThirtyAMInSeconds;
                int elapsedSeconds = secondsSinceMidnight - SixThirtyAMInSeconds;
                double percentage = (100 * elapsedSeconds / (double)totalSeconds);

                return ColorFromLightPercentage(percentage);
            }
            else if (secondsSinceMidnight > EightAMInSeconds && secondsSinceMidnight <= NineAMInSeconds)
            {
                // Between 8 AM and 9 AM: Steady bright white light
                return new AlarmClockColor(new LifxNet.Color { R = 0xFF, G = 0xFF, B = 0xFF }, MaxKelvin);
            }
            else
            {
                // After 9 AM: Off
                int secondsSinceNine = secondsSinceMidnight - NineAMInSeconds;
                byte dimmedValue = rgbValueForDim(secondsSinceNine);
                return new AlarmClockColor(new LifxNet.Color { R = dimmedValue, G = dimmedValue, B = dimmedValue }, MaxKelvin);
            }
        }

        private byte rgbValueForDim(int secondsSinceNine)
        {
            if (secondsSinceNine < 0) return 255;
            if (secondsSinceNine > 3600) return 0;
            return (byte)(255 - (int)(secondsSinceNine / 3600.0 * 255));
        }

        private static int GetSecondsSinceMidnight(DateTime time)
        {
            return time.Hour * 3600 + time.Minute * 60 + time.Second;
        }

        private static int SixThirtyAMInSeconds = (6 * 60 + 30) * 60;
        private static int EightAMInSeconds = (8 * 60) * 60;
        private static int NineAMInSeconds = (9 * 60) * 60;


        private static AlarmClockColor ColorFromLightPercentage(double percentage)
        {
            // Map percentage (0-100) to a color and kelvin value
            byte brightness = BrightnessFromPercentage(percentage);  // Scale 0-100 to 0-255
            ushort kelvin = KelvinFromPercentage(percentage);      // Scale 0-100 to 1500-6500K

            return new AlarmClockColor(new LifxNet.Color { R = brightness, G = brightness, B = brightness }, kelvin);
        }

        private static byte BrightnessFromPercentage(double percentage)
        {
            if (percentage < 0) return (byte)0x0;
            if (percentage > 50) return (byte)0xFF;
            return (byte)(percentage / 100.0 * 2 * 0xFF);
        }

        private static ushort KelvinFromPercentage(double percentage)
        {
            if (percentage < 25) return MinKelvin;
            if (percentage > 75) return MaxKelvin;
            const ushort range = MaxKelvin - MinKelvin;
            double multiplier = (percentage - 25) / 50.0;
            ushort addition = (ushort)(multiplier * range);
            return (ushort)(MinKelvin + addition);
        }
    }
}