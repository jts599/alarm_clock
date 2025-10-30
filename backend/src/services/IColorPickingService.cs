using System;
using Microsoft.Identity.Client;
namespace AlarmClock.Backend.Services
{
    public interface IAlarmClockColor
    {
        public LifxNet.Color Color { get; set; }
        public ushort Kelvin { get; set; }

        public Boolean IsEqual(IAlarmClockColor other)
        {
            return this.Color.R == other.Color.R &&
                   this.Color.G == other.Color.G &&
                   this.Color.B == other.Color.B
            && this.Kelvin == other.Kelvin;
        }
    }

    public class AlarmClockColor : IAlarmClockColor
    {
        public LifxNet.Color Color { get; set; }
        public ushort Kelvin { get; set; }

        public AlarmClockColor(LifxNet.Color color, ushort kelvin)
        {
            Color = color;
            Kelvin = kelvin;
        }

        public bool IsEqual(IAlarmClockColor other)
        {
            return this.Color.R == other.Color.R &&
                   this.Color.G == other.Color.G &&
                   this.Color.B == other.Color.B &&
                   this.Kelvin == other.Kelvin;
        }

        public override string ToString()
        {
            return $"Color(R:{Color.R}, G:{Color.G}, B:{Color.B}), Kelvin: {Kelvin}";
        }
    }

    public interface IColorPickingService
    {
        AlarmClockColor GetColorForTime(DateTime time);
        bool IsLightOnAtTime(DateTime time);
    }

}
