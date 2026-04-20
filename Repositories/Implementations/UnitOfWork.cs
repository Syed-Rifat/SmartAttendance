using System;
using System.Threading.Tasks;
using SmartAttendance.Data;
using SmartAttendance.Models.Entities;
using SmartAttendance.Repositories.Interfaces;

namespace SmartAttendance.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IRepository<User> Users { get; private set; }
        public IRepository<Student> Students { get; private set; }
        public IRepository<Teacher> Teachers { get; private set; }
        public IRepository<Course> Courses { get; private set; }
        public IRepository<CourseAssignment> CourseAssignments { get; private set; }
        public IRepository<Enrollment> Enrollments { get; private set; }
        public IRepository<AttendanceRecord> AttendanceRecords { get; private set; }
        public IRepository<ActivityLog> ActivityLogs { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            Students = new Repository<Student>(_context);
            Teachers = new Repository<Teacher>(_context);
            Courses = new Repository<Course>(_context);
            CourseAssignments = new Repository<CourseAssignment>(_context);
            Enrollments = new Repository<Enrollment>(_context);
            AttendanceRecords = new Repository<AttendanceRecord>(_context);
            ActivityLogs = new Repository<ActivityLog>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
