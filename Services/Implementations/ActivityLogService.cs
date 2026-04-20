using System;
using System.Threading.Tasks;
using SmartAttendance.Models.Entities;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Services.Interfaces;

namespace SmartAttendance.Services.Implementations
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IUnitOfWork _uow;

        public ActivityLogService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task LogActionAsync(int userId, string action, string description = null)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            await _uow.ActivityLogs.AddAsync(log);
            await _uow.SaveChangesAsync();
        }
    }
}
