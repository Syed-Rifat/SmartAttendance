using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Filters;
using System.Linq;
using SmartAttendance.Services.Interfaces;
using System.Collections.Generic;

namespace SmartAttendance.Controllers
{
    [Authorize(Roles = "Student")]
    [ServiceFilter(typeof(ActivityLogFilter))]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IAttendanceService _attendanceService;

        public StudentController(IUnitOfWork uow, IAttendanceService attendanceService)
        {
            _uow = uow;
            _attendanceService = attendanceService;
        }

        public async Task<IActionResult> MyAttendance()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var student = (await _uow.Students.FindAsync(s => s.UserId == userId)).FirstOrDefault();
            if (student == null) return NotFound();

            var enrollments = await _uow.Enrollments.FindAsync(e => e.StudentId == student.StudentId);
            
            var attendanceStats = new Dictionary<string, decimal>();

            foreach(var enrollment in enrollments)
            {
                var assignment = await _uow.CourseAssignments.GetByIdAsync(enrollment.AssignmentId);
                var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
                
                var pct = await _attendanceService.GetAttendancePercentageAsync(student.StudentId, assignment.AssignmentId);
                attendanceStats.Add($"{course.CourseName} - {assignment.Section}", pct);
            }

            return View(attendanceStats);
        }
    }
}
