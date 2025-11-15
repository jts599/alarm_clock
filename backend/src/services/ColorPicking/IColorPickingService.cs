using System;
using AlarmClock.Backend.DataModels.AlarmCore;
using Microsoft.Identity.Client;
namespace AlarmClock.Backend.Services
{

    public class AlarmClockColor
    {
        /// <summary>
        /// Lifx Color
        /// </summary>
        public LifxNet.Color Color { get; set; }

        /// <summary>
        /// Kelvin temperature
        /// </summary>
        public ushort Kelvin { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="color"> Lifx Color</param>
        /// <param name="kelvin">Kelvin temperature</param>
        public AlarmClockColor(LifxNet.Color color, ushort kelvin)
        {
            Color = color;
            Kelvin = kelvin;
        }

        public static AlarmClockColor Default => new AlarmClockColor(new LifxNet.Color { R = 0, G = 0, B = 0 }, 3500);

        /// <summary>
        /// Checks if this AlarmClockColor is equal to another
        /// </summary>
        /// <param name="other">The other AlarmClockColor to compare to</param>
        /// <returns>True if the colors and kelvin values are equal, false otherwise</returns>
        public bool IsEqual(AlarmClockColor other)
        {
            return this.Color.R == other.Color.R &&
                   this.Color.G == other.Color.G &&
                   this.Color.B == other.Color.B &&
                   this.Kelvin == other.Kelvin;
        }

        /// <summary>
        /// Returns a string representation of the AlarmClockColor
        /// </summary>
        /// <returns>A string representation of the AlarmClockColor</returns>
        public override string ToString()
        {
            return $"Color(R:{Color.R}, G:{Color.G}, B:{Color.B}), Kelvin: {Kelvin}";
        }
    }

    public interface IColorPickingService
    {
        /// <summary>
        /// Get the color for the given time
        /// </summary>
        /// <param name="time">The time to get the color for</param>
        /// <returns> A color for the lights to be set to</returns>
        AlarmClockColor GetColorForTime(DateTime time);

        /// <summary>
        /// Should return true if the light should be on at the given time
        /// </summary>
        /// <param name="time">The time to check</param>
        /// <returns>True if the light should be on, false otherwise</returns>
        bool IsLightOnAtTime(DateTime time);

        /// <summary>
        /// Should return a status message for the color picker 
        /// e.g. "On until 7:30 AM" or "Next Alarm at 6:30 AM"
        /// </summary>
        /// <returns></returns>
        AlarmEventInfo NextEvent(DateTime time);
    }

    public interface IOverrideColorPickingService : IColorPickingService
    {
        /// <summary>
        /// Unique identifier for this override
        /// </summary>
        string guid { get; }

        /// <summary>
        /// When should this override be active from
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// When should this override be active until
        /// </summary>
        DateTime EndTime { get; set; }
    }

    public interface ICompositeColorPickingService : IBaseColorPickingService
    {

        /// <summary>
        /// Add an override color picker
        /// </summary>
        /// <param name="colorPicker">The override color picker to add</param>
        void AddOverride(IOverrideColorPickingService colorPicker);

        void ClearOverrideByGuid(string overrideGuid);

        void ClearAllOverrides();

        int GetOverrideCount();

        LightOverrideState GetCurrentOverride();

        ICompositeColorPickingService ReconstructWithBase(IBaseColorPickingService baseColorPicker);
        new ICompositeColorPickingService Clone();
    }

    public interface IConfigurableColorPickingServiceParameters
    {
        TimeOnly AlarmTime { get; }
        int TransitionMinutes { get; }
        int HoldOnMinutes { get; }
        DayOfWeek[] ActiveDays { get; }
    }

    public interface IBaseColorPickingService : IColorPickingService
    {
        IBaseColorPickingService Clone();
        IConfigurableColorPickingServiceParameters GetParameters();
    }

}
