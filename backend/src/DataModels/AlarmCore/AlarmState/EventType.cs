namespace AlarmClock.Backend.DataModels.AlarmCore
{
    /// <summary>
    /// Types of alarm events. 0 Corresponds to Light Off, 1 corresponds to Sunrise.
    /// </summary>
    public enum EventType
    {
        //Light Off
        LightOff = 0,

        //Light On
        Sunrise = 1
    }
}
