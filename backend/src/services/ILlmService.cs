using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public interface ILlmService
    {
        Task<string> SelectWeatherIconAsync(string weatherDescription);
        Task<string> GenerateResponseAsync(string prompt);
    }
}