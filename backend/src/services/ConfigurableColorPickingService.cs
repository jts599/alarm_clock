using System;
using System.Collections.Generic;
using LifxNet;

namespace MyFullstackApp.Services
{

    public class ConfigurableColorPickingServiceConstructionParameters
    {
        public TimeOnly AlarmTime { get; }
        public int TransitionMinutes { get; }
        public int HoldOnMinutes { get; }
        public DayOfWeek[] ActiveDays { get; }

        public bool Validate(out string errorMessage)
        {
            if (AlarmTime == default)
            {
                errorMessage = "AlarmTime must be set.";
                return false;
            }
            if (TransitionMinutes <= 0)
            {
                errorMessage = "TransitionMinutes must be greater than 0.";
                return false;
            }
            if (HoldOnMinutes < 0)
            {
                errorMessage = "HoldOnMinutes cannot be negative.";
                return false;
            }
            if (ActiveDays == null || ActiveDays.Length == 0)
            {
                errorMessage = "ActiveDays must contain at least one day.";
                return false;
            }

            DateTime today = DateTime.Today;
            DateTime alarmDateTime = today.AddHours(AlarmTime.Hour).AddMinutes(AlarmTime.Minute);
            if (alarmDateTime.AddMinutes(TransitionMinutes + HoldOnMinutes).Day != today.Day)
            {
                errorMessage = "The combination of AlarmTime, TransitionMinutes, and HoldOnMinutes must not cross over to the next day.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }

    public class ConfigurableColorPickingService : IColorPickingService
    {

        public ConfigurableColorPickingService(ConfigurableColorPickingServiceConstructionParameters parameters)
            : this(parameters.AlarmTime, parameters.TransitionMinutes, parameters.HoldOnMinutes, parameters.ActiveDays)
        {
            if (!parameters.Validate(out string errorMessage))
            {
                throw new ArgumentException($"Invalid construction parameters: {errorMessage}");
            }
        }

        private ConfigurableColorPickingService(TimeOnly alarmTime, int transitionMinutes, int holdOnMinutes, DayOfWeek[] activeDays)
        {
            AlarmStartTimeInSeconds = (alarmTime.Hour * 60 + alarmTime.Minute) * 60;
            EightAMInSeconds = AlarmStartTimeInSeconds + (transitionMinutes * 60);
            NineAMInSeconds = EightAMInSeconds + (holdOnMinutes * 60);
            ActiveDays = [.. activeDays];
        }

        private const int MaxKelvin = 4500;
        private const int MinKelvin = 1500;


        /// <summary>
        /// The days of the week the alarm is active on.
        /// </summary>
        private HashSet<DayOfWeek> ActiveDays { get; set; }
        /// <summary>
        /// Sunrise start time: 6:30 AM
        /// </summary>
        private int AlarmStartTimeInSeconds = (6 * 60 + 30) * 60;

        /// <summary>
        /// Sunrise end time: 8:00 AM
        /// </summary>
        private int EightAMInSeconds = (8 * 60) * 60;

        /// <summary>
        /// Lights are held on until: 9:00 AM
        /// </summary>
        private int NineAMInSeconds = (9 * 60) * 60;

        /// <summary>
        /// Determines if the light should be on at the given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsLightOnAtTime(DateTime time)
        {
            int secondsSinceMidnight = GetSecondsSinceMidnight(time);
            if (IsDuringTransitionToOnTime(secondsSinceMidnight) ||
                IsDuringHoldOnTime(secondsSinceMidnight))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Gets a status string for the given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string Status(DateTime time)
        {
            int secondsSinceMidnight = GetSecondsSinceMidnight(time);
            if (IsDuringLightsOffTime(secondsSinceMidnight))
            {
                return "Lights turn on at 6:30 AM";
            }
            else if (IsDuringTransitionToOnTime(secondsSinceMidnight))
            {
                return "Sun is rising";
            }
            else if (IsDuringHoldOnTime(secondsSinceMidnight))
            {
                return "Lights off at 9:00 AM";
            }
            return "Lights are off";
        }

        /// <summary>
        /// Determines if the current time is during the lights off period.
        /// </summary>
        /// <param name="secondsSinceMidnight"></param>
        /// <returns></returns>
        private bool IsDuringLightsOffTime(int secondsSinceMidnight)
        {
            if (!ActiveDays.Contains(DateTime.Now.DayOfWeek))
            {
                return true;
            }
            if (secondsSinceMidnight < AlarmStartTimeInSeconds)
            {
                return true;
            }
            if (secondsSinceMidnight > NineAMInSeconds)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Determines if the current time is during the transition to on period. (Sunrise)
        /// </summary>
        /// <param name="secondsSinceMidnight"></param>
        /// <returns></returns>
        private bool IsDuringTransitionToOnTime(int secondsSinceMidnight)
        {
            if (!ActiveDays.Contains(DateTime.Now.DayOfWeek))
            {
                return false;
            }
            return secondsSinceMidnight >= AlarmStartTimeInSeconds && secondsSinceMidnight <= EightAMInSeconds;
        }

        /// <summary>
        /// Determines if the current time is during the hold on period. (Daylight)
        /// </summary>
        /// <param name="secondsSinceMidnight"></param>
        /// <returns></returns>
        private bool IsDuringHoldOnTime(int secondsSinceMidnight)
        {
            if (!ActiveDays.Contains(DateTime.Now.DayOfWeek))
            {
                return false;
            }
            return secondsSinceMidnight > EightAMInSeconds && secondsSinceMidnight <= NineAMInSeconds;
        }

        /// <summary>
        /// Gets the color for the given time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public AlarmClockColor GetColorForTime(DateTime time)
        {
            int secondsSinceMidnight = GetSecondsSinceMidnight(time);

            if (IsDuringLightsOffTime(secondsSinceMidnight))
            {
                // Before 6 AM: Off
                return new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, MinKelvin);

            }
            else if (IsDuringTransitionToOnTime(secondsSinceMidnight))
            {
                // Between 6 AM and 8 AM: Gradually increase brightness and color temperature
                int totalSeconds = EightAMInSeconds - AlarmStartTimeInSeconds;
                int elapsedSeconds = secondsSinceMidnight - AlarmStartTimeInSeconds;
                double percentage = (100 * elapsedSeconds / (double)totalSeconds);

                return ColorFromLightPercentage(percentage);
            }
            else if (IsDuringHoldOnTime(secondsSinceMidnight))
            {
                // Between 8 AM and 9 AM: Steady bright white light
                return new AlarmClockColor(new LifxNet.Color { R = 0xFF, G = 0xFF, B = 0xFF }, MaxKelvin);
            }
            //Default: Off
            return new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, MinKelvin);
        }

        /// <summary>
        /// Gets the number of seconds since midnight for the given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static int GetSecondsSinceMidnight(DateTime time)
        {
            return time.Hour * 3600 + time.Minute * 60 + time.Second;
        }

        /// <summary>
        /// Gets a color from a light percentage (0-100).
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        private static AlarmClockColor ColorFromLightPercentage(double percentage)
        {
            // Map percentage (0-100) to a color and kelvin value
            byte brightness = BrightnessFromPercentage(percentage);  // Scale 0-100 to 0-255
            ushort kelvin = KelvinFromPercentage(percentage);      // Scale 0-100 to 1500-6500K

            return new AlarmClockColor(new LifxNet.Color { R = brightness, G = brightness, B = brightness }, kelvin);
        }

        /// <summary>
        /// Gets brightness (0-255) from percentage (0-100).
        /// Starts at 0 at 0% and goes to 255 at 50%, then stays at 255.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        private static byte BrightnessFromPercentage(double percentage)
        {
            if (percentage < 0) return (byte)0x0;
            if (percentage > 50) return (byte)0xFF;
            return (byte)(percentage / 100.0 * 2 * 0xFF);
        }

        /// <summary>
        /// Gets kelvin temperature from percentage (MinKelvin- MaxKelvin).
        /// Starts at MinKelvin at 25% and goes to MaxKelvin at 75%
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
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