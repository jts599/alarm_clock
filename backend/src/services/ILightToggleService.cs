using System.Threading.Tasks;

namespace MyFullstackApp.Services
{
    public interface ILightToggleService
    {
        Task StartAsync();
        Task StopAsync();
        bool IsRunning { get; }
    }
}