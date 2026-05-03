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
            var assignmentList = assignments.ToList();

            var vm = new TeacherOverviewViewModel
            {
                TeacherName = teacher.FullName,
                Department = teacher.Department ?? "",
                TotalCourses = assignmentList.Count
            };

            int totalStudents = 0;
            var today = DateTime.Today.DayOfWeek.ToString(); // "Sunday", "Monday", etc.
            var assignmentIds = assignmentList.Select(a => a.AssignmentId).ToList();
            
            // Get today's schedules
            var todaySchedules = await _uow.ClassSchedules.FindAsync(cs => 
                assignmentIds.Contains(cs.AssignmentId) && cs.DayOfWeek == today);

            foreach (var a in assignmentList)
            {
                var enrollments = await _uow.Enrollments.FindAsync(e => e.AssignmentId == a.AssignmentId);
                totalStudents += enrollments.Count();
            }
            
            // Map today's schedules to view models
            foreach (var schedule in todaySchedules.OrderBy(s => s.StartTime))
            {
                var assignment = assignmentList.First(a => a.AssignmentId == schedule.AssignmentId);
                var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
                
                vm.TodayClasses.Add(new ScheduleItemViewModel
                {
                    CourseName = course?.CourseName ?? "",
                    CourseCode = course?.CourseCode ?? "",
                    Section = assignment.Section,
                    Room = assignment.Room ?? "",
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime
                });
            }

            vm.TotalStudents = totalStudents;
            return View(vm);
        }

        public async Task<IActionResult> MyCourses()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var teacher = (await _uow.Teachers.FindAsync(t => t.UserId == userId)).FirstOrDefault();
            
            if (teacher == null) return NotFound();

            var assignments = await _uow.CourseAssignments.FindAsync(ca => ca.TeacherId == teacher.TeacherId);
            var assignmentList = assignments.ToList();

            var vm = new TeacherDashboardViewModel
            {
                TeacherName = teacher.FullName,
                Department = teacher.Department ?? "",
                TotalCourses = assignmentList.Count
            };

            int totalStudents = 0;
            foreach (var a in assignmentList)
            {
                var course = await _uow.Courses.GetByIdAsync(a.CourseId);
                var enrollments = await _uow.Enrollments.FindAsync(e => e.AssignmentId == a.AssignmentId);
                var enrolledCount = enrollments.Count();
                totalStudents += enrolledCount;

                vm.Courses.Add(new TeacherCourseItem
                {
                    AssignmentId = a.AssignmentId,
                    CourseName = course?.CourseName ?? "",
                    CourseCode = course?.CourseCode ?? "",
                    Section = a.Section,
                    Semester = a.Semester,
                    Room = a.Room,
                    Schedule = a.Schedule,
                    EnrolledStudentCount = enrolledCount
                });
            }

            vm.TotalStudents = totalStudents;
            return View(vm);
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

        [HttpGet]
        public async Task<IActionResult> AttendanceHistory(int assignmentId)
        {
            var assignment = await _uow.CourseAssignments.GetByIdAsync(assignmentId);
            if (assignment == null) return NotFound();

            var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
            var enrollments = await _uow.Enrollments.FindAsync(e => e.AssignmentId == assignmentId);

            var vm = new TeacherAttendanceSummaryViewModel
            {
                AssignmentId = assignmentId,
                CourseName = course?.CourseName,
                CourseCode = course?.CourseCode,
                Section = assignment.Section,
                Semester = assignment.Semester
            };

            foreach (var e in enrollments)
            {
                var student = await _uow.Students.GetByIdAsync(e.StudentId);
                var records = await _uow.AttendanceRecords.FindAsync(a => a.EnrollmentId == e.EnrollmentId && a.AssignmentId == assignmentId);
                var recordList = records.ToList();
                var totalClasses = recordList.Count;
                var attended = recordList.Count(r => r.Status == "Present" || r.Status == "Late");
                var pct = totalClasses > 0 ? Math.Round((decimal)attended / totalClasses * 100, 1) : 0;

                vm.Students.Add(new StudentSummaryItem
                {
                    StudentId = student?.StudentId ?? 0,
                    StudentCode = student?.StudentCode,
                    FullName = student?.FullName,
                    TotalClasses = totalClasses,
                    Attended = attended,
                    Percentage = pct,
                    IsLow = pct < 75
                });
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> MySchedule()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var teacher = (await _uow.Teachers.FindAsync(t => t.UserId == userId)).FirstOrDefault();
            if (teacher == null) return NotFound();

            var assignments = await _uow.CourseAssignments.FindAsync(ca => ca.TeacherId == teacher.TeacherId);
            var scheduleItems = new List<ScheduleItemViewModel>();

            foreach (var a in assignments)
            {
                var course = await _uow.Courses.GetByIdAsync(a.CourseId);
                var slots = await _uow.ClassSchedules.FindAsync(cs => cs.AssignmentId == a.AssignmentId);

                // If slots exist, add one item per slot
                if (slots.Any())
                {
                    foreach (var slot in slots)
                    {
                        scheduleItems.Add(new ScheduleItemViewModel
                        {
                            CourseName = course?.CourseName,
                            CourseCode = course?.CourseCode,
                            Section = a.Section,
                            Semester = a.Semester,
                            Room = a.Room,
                            Schedule = a.Schedule, // legacy field
                            DayOfWeek = slot.DayOfWeek,
                            StartTime = slot.StartTime,
                            EndTime = slot.EndTime
                        });
                    }
                }
                else
                {
                    // Fallback for assignments without slots
                    scheduleItems.Add(new ScheduleItemViewModel
                    {
                        CourseName = course?.CourseName,
                        CourseCode = course?.CourseCode,
                        Section = a.Section,
                        Semester = a.Semester,
                        Room = a.Room,
                        Schedule = a.Schedule
                    });
                }
            }

            return View(scheduleItems);
        }
    }
}
