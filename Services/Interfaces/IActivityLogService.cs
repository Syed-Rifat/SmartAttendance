using System.Threading.Tasks;

namespace SmartAttendance.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogActionAsync(int userId, string action, string description = null);
    }
}
