using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SmartAttendance.Models.ViewModels;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Filters;
using System.Linq;
using SmartAttendance.Services.Interfaces;
using System.Security.Claims;
using System;

namespace SmartAttendance.Controllers
{
    [Authorize(Roles = "Teacher")]
    [ServiceFilter(typeof(ActivityLogFilter))]
    public class TeacherController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IAttendanceService _attendanceService;

        public TeacherController(IUnitOfWork uow, IAttendanceService attendanceService)
        {
            _uow = uow;
            _attendanceService = attendanceService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var teacher = (await _uow.Teachers.FindAsync(t => t.UserId == userId)).FirstOrDefault();
            
            if (teacher == null) return NotFound();

            var assignments = await _uow.CourseAssignments.FindAsync(ca => ca.TeacherId == teacher.TeacherId);
            return View(assignments);
        }

        [HttpGet]
        public async Task<IActionResult> TakeAttendance(int assignmentId)
        {
            var assignment = await _uow.CourseAssignments.GetByIdAsync(assignmentId);
            if (assignment == null) return NotFound();

            var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
            var enrollments = await _uow.Enrollments.FindAsync(e => e.AssignmentId == assignmentId);
            
            var vm = new AttendanceViewModel
            {
                AssignmentId = assignmentId,
                CourseName = course?.CourseName,
                Section = assignment.Section,
                AttendanceDate = DateOnly.FromDateTime(DateTime.Today)
            };

            foreach(var e in enrollments)
            {
                var student = await _uow.Students.GetByIdAsync(e.StudentId);
                var record = (await _uow.AttendanceRecords.FindAsync(a => a.EnrollmentId == e.EnrollmentId && a.AttendanceDate == vm.AttendanceDate)).FirstOrDefault();
                
                vm.Students.Add(new StudentAttendanceItem
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    StudentCode = student?.StudentCode,
                    FullName = student?.FullName,
                    Status = record?.Status ?? "Present" // Default to Present
                });
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> TakeAttendance(AttendanceViewModel model)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var teacher = (await _uow.Teachers.FindAsync(t => t.UserId == userId)).FirstOrDefault();
            
            foreach(var student in model.Students)
            {
                await _attendanceService.TakeAttendanceAsync(model.AssignmentId, model.AttendanceDate, teacher.TeacherId, student.EnrollmentId, student.Status);
            }

            TempData["Message"] = "Attendance recorded successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> EditAttendance(int assignmentId, string date)
        {
            var parsedDate = DateOnly.Parse(date);
            
            // Validate time limit (e.g., within 24 hours)
            if (parsedDate < DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
            {
                TempData["Message"] = "You can only edit attendance within the last 24 hours.";
                return RedirectToAction(nameof(Dashboard));
            }

            var assignment = await _uow.CourseAssignments.GetByIdAsync(assignmentId);
            if (assignment == null) return NotFound();

            var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
            var enrollments = await _uow.Enrollments.FindAsync(e => e.AssignmentId == assignmentId);
            
            var vm = new AttendanceViewModel
            {
                AssignmentId = assignmentId,
                CourseName = course?.CourseName,
                Section = assignment.Section,
                AttendanceDate = parsedDate
            };

            foreach(var e in enrollments)
            {
                var student = await _uow.Students.GetByIdAsync(e.StudentId);
                var record = (await _uow.AttendanceRecords.FindAsync(a => a.EnrollmentId == e.EnrollmentId && a.AttendanceDate == vm.AttendanceDate)).FirstOrDefault();
                
                vm.Students.Add(new StudentAttendanceItem
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    StudentCode = student?.StudentCode,
                    FullName = student?.FullName,
                    Status = record?.Status ?? "Present"
                });
            }

            return View("TakeAttendance", vm); // Reuse the same view
        }

        [HttpGet]
        public async Task<IActionResult> LowAttendance(int assignmentId)
        {
            var students = await _attendanceService.GetLowAttendanceStudentsAsync(assignmentId);
            var assignment = await _uow.CourseAssignments.GetByIdAsync(assignmentId);
            var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);

            var vmList = new List<StudentSummaryItem>();
            foreach(var s in students)
            {
                var pct = await _attendanceService.GetAttendancePercentageAsync(s.StudentId, assignmentId);
                vmList.Add(new StudentSummaryItem
                {
                    StudentId = s.StudentId,
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    Percentage = pct,
                    IsLow = true
                });
            }

            ViewBag.CourseInfo = $"{course.CourseName} - {assignment.Section}";
            return View(vmList);
        }

        [HttpGet]
        public async Task<IActionResult> ExportAttendance(int assignmentId)
        {
            // Injecting IReportService via request services because it wasn't in constructor to keep changes small
            var reportService = HttpContext.RequestServices.GetService(typeof(IReportService)) as IReportService;
            var fileBytes = await reportService.ExportAttendanceReportAsync(assignmentId, "excel");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Attendance_{assignmentId}.xlsx");
        }
    }
}
