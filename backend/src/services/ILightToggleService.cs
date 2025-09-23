using System.Threading.Tasks;

namespace MyFullstackApp.Services
{
    public interface ILightToggleService
    {
        bool IsRunning { get; }
    }
}