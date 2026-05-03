using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Filters;
using System.Linq;
using SmartAttendance.Services.Interfaces;
using System.Collections.Generic;
using SmartAttendance.Models.ViewModels;
using System;

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

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var student = (await _uow.Students.FindAsync(s => s.UserId == userId)).FirstOrDefault();
            if (student == null) return NotFound();

            var enrollments = await _uow.Enrollments.FindAsync(e => e.StudentId == student.StudentId);
            var enrollmentList = enrollments.ToList();

            var vm = new StudentDashboardViewModel
            {
                StudentName = student.FullName,
                StudentCode = student.StudentCode,
                Department = student.Department ?? "",
                Batch = student.Batch ?? "",
                TotalEnrolledCourses = enrollmentList.Count
            };

            int totalPresent = 0, totalClasses = 0, lowCount = 0;

            foreach (var enrollment in enrollmentList)
            {
                var assignment = await _uow.CourseAssignments.GetByIdAsync(enrollment.AssignmentId);
                var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
                var records = await _uow.AttendanceRecords.FindAsync(a => a.EnrollmentId == enrollment.EnrollmentId && a.AssignmentId == assignment.AssignmentId);
                var recordList = records.ToList();

                var classCount = recordList.Count;
                var present = recordList.Count(r => r.Status == "Present" || r.Status == "Late");
                var pct = classCount > 0 ? Math.Round((decimal)present / classCount * 100, 1) : 0;
                var isLow = pct < 75;

                totalClasses += classCount;
                totalPresent += present;
                if (isLow && classCount > 0) lowCount++;

                vm.CourseStats.Add(new StudentCourseStatItem
                {
                    CourseName = course?.CourseName ?? "",
                    CourseCode = course?.CourseCode ?? "",
                    Section = assignment.Section,
                    Semester = assignment.Semester,
                    AttendancePercentage = pct,
                    TotalClasses = classCount,
                    PresentCount = present,
                    IsLow = isLow
                });
            }

            vm.TotalClassesConducted = totalClasses;
            vm.TotalClassesAttended = totalPresent;
            vm.OverallAttendancePercentage = totalClasses > 0 ? Math.Round((decimal)totalPresent / totalClasses * 100, 1) : 0;
            vm.LowAttendanceCourseCount = lowCount;

            // Fetch Today's Classes
            var today = DateTime.Today.DayOfWeek.ToString();
            var assignmentIds = enrollmentList.Select(e => e.AssignmentId).ToList();
            var todaySchedules = await _uow.ClassSchedules.FindAsync(cs => 
                assignmentIds.Contains(cs.AssignmentId) && cs.DayOfWeek == today);

            foreach (var schedule in todaySchedules.OrderBy(s => s.StartTime))
            {
                var assignment = await _uow.CourseAssignments.GetByIdAsync(schedule.AssignmentId);
                var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);

                vm.TodayClasses.Add(new ScheduleItemViewModel
                {
                    CourseName = course?.CourseName ?? "",
                    CourseCode = course?.CourseCode ?? "",
                    Section = assignment?.Section ?? "",
                    Room = assignment?.Room ?? "",
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime
                });
            }

            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> AttendanceDetails()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var student = (await _uow.Students.FindAsync(s => s.UserId == userId)).FirstOrDefault();
            if (student == null) return NotFound();

            var vm = new StudentAttendanceDetailViewModel
            {
                StudentName = student.FullName,
                StudentCode = student.StudentCode,
                Department = student.Department ?? "N/A",
                Batch = student.Batch ?? "N/A"
            };

            var enrollments = await _uow.Enrollments.FindAsync(e => e.StudentId == student.StudentId);

            foreach (var enrollment in enrollments)
            {
                var assignment = await _uow.CourseAssignments.GetByIdAsync(enrollment.AssignmentId);
                var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
                var records = await _uow.AttendanceRecords.FindAsync(a => a.EnrollmentId == enrollment.EnrollmentId && a.AssignmentId == assignment.AssignmentId);
                var recordList = records.ToList();

                var totalClasses = recordList.Count;
                var presentCount = recordList.Count(r => r.Status == "Present");
                var absentCount = recordList.Count(r => r.Status == "Absent");
                var lateCount = recordList.Count(r => r.Status == "Late");
                var pct = totalClasses > 0 ? Math.Round((decimal)(presentCount + lateCount) / totalClasses * 100, 1) : 0;

                var courseInfo = new CourseAttendanceInfo
                {
                    CourseName = course?.CourseName,
                    CourseCode = course?.CourseCode,
                    Section = assignment.Section,
                    Semester = assignment.Semester,
                    TotalClasses = totalClasses,
                    PresentCount = presentCount,
                    AbsentCount = absentCount,
                    LateCount = lateCount,
                    AttendancePercentage = pct,
                    IsLowAttendance = pct < 75,
                    Records = recordList.OrderByDescending(r => r.AttendanceDate).Select(r => new AttendanceRecordDetail
                    {
                        Date = r.AttendanceDate,
                        Status = r.Status
                    }).ToList()
                };

                vm.Courses.Add(courseInfo);
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> MyTimetable()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var student = (await _uow.Students.FindAsync(s => s.UserId == userId)).FirstOrDefault();
            if (student == null) return NotFound();

            var enrollments = await _uow.Enrollments.FindAsync(e => e.StudentId == student.StudentId);
            var scheduleItems = new List<ScheduleItemViewModel>();

            foreach (var enrollment in enrollments)
            {
                var assignment = await _uow.CourseAssignments.GetByIdAsync(enrollment.AssignmentId);
                var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
                var teacher = await _uow.Teachers.GetByIdAsync(assignment.TeacherId);
                
                var slots = await _uow.ClassSchedules.FindAsync(cs => cs.AssignmentId == assignment.AssignmentId);

                // If slots exist, add one item per slot
                if (slots.Any())
                {
                    foreach (var slot in slots)
                    {
                        scheduleItems.Add(new ScheduleItemViewModel
                        {
                            CourseName = course?.CourseName,
                            CourseCode = course?.CourseCode,
                            Section = assignment.Section,
                            Semester = assignment.Semester,
                            Room = assignment.Room,
                            Schedule = assignment.Schedule, // legacy field
                            TeacherName = teacher?.FullName,
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
                        Section = assignment.Section,
                        Semester = assignment.Semester,
                        Room = assignment.Room,
                        Schedule = assignment.Schedule,
                        TeacherName = teacher?.FullName
                    });
                }
            }

            return View(scheduleItems);
        }
    }
}
