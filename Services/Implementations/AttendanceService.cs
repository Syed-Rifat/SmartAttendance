using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartAttendance.Models.Entities;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Services.Interfaces;
using SmartAttendance.Patterns.Singleton;

namespace SmartAttendance.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly AppSettings _appSettings;

        public AttendanceService(IUnitOfWork uow, AppSettings appSettings)
        {
            _uow = uow;
            _appSettings = appSettings;
        }

        public async Task TakeAttendanceAsync(int assignmentId, DateOnly date, int markedByTeacherId, int enrollmentId, string status)
        {
            // Design by Contract Guard Clauses
            if (date > DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentException("Cannot record attendance for a future date.");

            if (!new[] { "Present", "Absent", "Late" }.Contains(status))
                throw new ArgumentException("Invalid attendance status.");

            var existing = (await _uow.AttendanceRecords.FindAsync(a => a.EnrollmentId == enrollmentId && a.AttendanceDate == date)).FirstOrDefault();

            if (existing != null)
            {
                // Update
                existing.Status = status;
                existing.EditedAt = DateTime.UtcNow;
                _uow.AttendanceRecords.Update(existing);
            }
            else
            {
                // Insert
                var record = new AttendanceRecord
                {
                    EnrollmentId = enrollmentId,
                    AssignmentId = assignmentId,
                    AttendanceDate = date,
                    Status = status,
                    MarkedByTeacherId = markedByTeacherId
                };
                await _uow.AttendanceRecords.AddAsync(record);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task<decimal> GetAttendancePercentageAsync(int studentId, int assignmentId)
        {
            var records = await _uow.AttendanceRecords.FindAsync(a => a.Enrollment.StudentId == studentId && a.AssignmentId == assignmentId);
            
            if (!records.Any()) return 0;
            
            var attended = records.Count(r => r.Status == "Present" || r.Status == "Late");
            return Math.Round((decimal)attended / records.Count() * 100, 2);
        }

        public async Task<IEnumerable<Student>> GetLowAttendanceStudentsAsync(int assignmentId)
        {
            var threshold = _appSettings.MinAttendanceThreshold;
            var enrollments = await _uow.Enrollments.FindAsync(e => e.AssignmentId == assignmentId);
            
            var lowAttendanceStudents = new List<Student>();
            
            foreach(var enrollment in enrollments)
            {
                var pct = await GetAttendancePercentageAsync(enrollment.StudentId, assignmentId);
                if (pct < threshold)
                {
                    var student = await _uow.Students.GetByIdAsync(enrollment.StudentId);
                    if (student != null)
                        lowAttendanceStudents.Add(student);
                }
            }
            
            return lowAttendanceStudents;
        }
    }
}
