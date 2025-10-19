using System.Threading.Tasks;
using LifxNet;

namespace MyFullstackApp.Services
{
    public interface ILifxService
    {
        Task<int> GetNumberOfBulbsAsync();
        Task<bool> SetAllBulbsPowerAsync(bool powerOn);
        Task<bool> SetColorAllAsync(Color color, ushort kelvin, int transitionTime = 0);
        Task InitializeAsync();
    }
}