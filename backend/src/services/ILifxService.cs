using System.Threading.Tasks;

namespace MyFullstackApp.Services
{
    public interface ILifxService
    {
        Task<int> GetNumberOfBulbsAsync();
        Task<bool> SetAllBulbsPowerAsync(bool powerOn);
        Task InitializeAsync();
    }
}