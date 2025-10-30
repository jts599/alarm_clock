using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public interface ILightToggleService
    {
        bool IsRunning { get; }
    }
}