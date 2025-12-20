
using System.Threading.Tasks;
using AlarmClock.Backend.DataModels.AlarmCore;

namespace AlarmClock.Backend.Services
{
    public interface IStateSummaryService
    {
        Task<AlarmStateSummary> GetAlarmStateSummaryAsync();
    }
}