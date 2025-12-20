namespace AlarmClock.Backend.DataModels.AlarmCore
{
    public class LightOverrideState
    {
        public string OverrideGuid { get; set; } = string.Empty;

        public bool IsLightCurrentlyOn { get; set; } = true;
    }
}
