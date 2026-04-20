using System;
using System.Threading.Tasks;
using SmartAttendance.Models.Entities;

namespace SmartAttendance.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Student> Students { get; }
        IRepository<Teacher> Teachers { get; }
        IRepository<Course> Courses { get; }
        IRepository<CourseAssignment> CourseAssignments { get; }
        IRepository<Enrollment> Enrollments { get; }
        IRepository<AttendanceRecord> AttendanceRecords { get; }
        IRepository<ActivityLog> ActivityLogs { get; }

        Task<int> SaveChangesAsync();
    }
}
