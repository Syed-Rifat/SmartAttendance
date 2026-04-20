using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartAttendance.Models.Entities;

namespace SmartAttendance.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task TakeAttendanceAsync(int assignmentId, DateOnly date, int markedByTeacherId, int enrollmentId, string status);
        Task<decimal> GetAttendancePercentageAsync(int studentId, int assignmentId);
        Task<IEnumerable<Student>> GetLowAttendanceStudentsAsync(int assignmentId);
    }
}
