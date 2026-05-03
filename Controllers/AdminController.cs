using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SmartAttendance.Models.ViewModels;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Filters;
using System.Linq;
using SmartAttendance.Services.Interfaces;
using SmartAttendance.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SmartAttendance.Controllers
{
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(ActivityLogFilter))]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IReportService _reportService;

        public AdminController(IUnitOfWork uow, IReportService reportService)
        {
            _uow = uow;
            _reportService = reportService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var students = await _uow.Students.GetAllAsync();
            var teachers = await _uow.Teachers.GetAllAsync();
            var courses = await _uow.Courses.GetAllAsync();
            var today = System.DateOnly.FromDateTime(System.DateTime.Today);
            var attendanceToday = await _uow.AttendanceRecords.FindAsync(a => a.AttendanceDate == today);

            var vm = new DashboardViewModel
            {
                TotalStudents = students.Count(),
                TotalTeachers = teachers.Count(),
                TotalCourses = courses.Count(),
                TodayAttendanceCount = attendanceToday.Count()
            };

            return View(vm);
        }

        // ================= STUDENT CRUD =================

        public async Task<IActionResult> Students()
        {
            var students = await _uow.Students.GetAllAsync();
            var vmList = students.Select(s => new StudentViewModel
            {
                StudentId = s.StudentId,
                StudentCode = s.StudentCode,
                FullName = s.FullName,
                Department = s.Department,
                Batch = s.Batch,
                Section = s.Section
            }).ToList();

            return View(vmList);
        }

        [HttpGet]
        public IActionResult CreateStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = (await _uow.Users.FindAsync(u => u.Username == model.Username || u.Email == model.Email)).FirstOrDefault();
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username or Email already exists.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "Student"
                };

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var student = new Student
                {
                    UserId = user.UserId,
                    StudentCode = model.StudentCode,
                    FullName = model.FullName,
                    Department = model.Department,
                    Batch = model.Batch,
                    Section = model.Section,
                    ContactNumber = model.ContactNumber
                };

                await _uow.Students.AddAsync(student);
                await _uow.SaveChangesAsync();

                TempData["Message"] = "Student created successfully.";
                return RedirectToAction(nameof(Students));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _uow.Students.GetByIdAsync(id);
            if (student == null) return NotFound();

            var vm = new StudentEditViewModel
            {
                StudentId = student.StudentId,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Department = student.Department,
                Batch = student.Batch,
                Section = student.Section,
                ContactNumber = student.ContactNumber
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = await _uow.Students.GetByIdAsync(model.StudentId);
                if (student == null) return NotFound();

                student.StudentCode = model.StudentCode;
                student.FullName = model.FullName;
                student.Department = model.Department;
                student.Batch = model.Batch;
                student.Section = model.Section;
                student.ContactNumber = model.ContactNumber;

                _uow.Students.Update(student);
                await _uow.SaveChangesAsync();

                TempData["Message"] = "Student updated successfully.";
                return RedirectToAction(nameof(Students));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _uow.Students.GetByIdAsync(id);
            if (student != null)
            {
                // Cascade delete via User
                var user = await _uow.Users.GetByIdAsync(student.UserId);
                if (user != null)
                {
                    _uow.Users.Remove(user);
                    await _uow.SaveChangesAsync();
                }
                TempData["Message"] = "Student deleted successfully.";
            }
            return RedirectToAction(nameof(Students));
        }

        // ================= TEACHER CRUD =================

        public async Task<IActionResult> Teachers()
        {
            var teachers = await _uow.Teachers.GetAllAsync();
            var vmList = new System.Collections.Generic.List<TeacherListViewModel>();
            
            foreach(var t in teachers)
            {
                var user = await _uow.Users.GetByIdAsync(t.UserId);
                vmList.Add(new TeacherListViewModel
                {
                    TeacherId = t.TeacherId,
                    FullName = t.FullName,
                    Department = t.Department,
                    ContactNumber = t.ContactNumber,
                    Username = user?.Username,
                    Email = user?.Email
                });
            }

            return View(vmList);
        }

        [HttpGet]
        public IActionResult CreateTeacher()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = (await _uow.Users.FindAsync(u => u.Username == model.Username || u.Email == model.Email)).FirstOrDefault();
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username or Email already exists.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "Teacher"
                };

                await _uow.Users.AddAsync(user);
                await _uow.SaveChangesAsync();

                var teacher = new Teacher
                {
                    UserId = user.UserId,
                    FullName = model.FullName,
                    Department = model.Department,
                    ContactNumber = model.ContactNumber
                };

                await _uow.Teachers.AddAsync(teacher);
                await _uow.SaveChangesAsync();

                TempData["Message"] = "Teacher created successfully.";
                return RedirectToAction(nameof(Teachers));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _uow.Teachers.GetByIdAsync(id);
            if (teacher == null) return NotFound();

            var vm = new TeacherEditViewModel
            {
                TeacherId = teacher.TeacherId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                ContactNumber = teacher.ContactNumber
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditTeacher(TeacherEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var teacher = await _uow.Teachers.GetByIdAsync(model.TeacherId);
                if (teacher == null) return NotFound();

                teacher.FullName = model.FullName;
                teacher.Department = model.Department;
                teacher.ContactNumber = model.ContactNumber;

                _uow.Teachers.Update(teacher);
                await _uow.SaveChangesAsync();

                TempData["Message"] = "Teacher updated successfully.";
                return RedirectToAction(nameof(Teachers));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _uow.Teachers.GetByIdAsync(id);
            if (teacher != null)
            {
                var user = await _uow.Users.GetByIdAsync(teacher.UserId);
                if (user != null)
                {
                    _uow.Users.Remove(user);
                    await _uow.SaveChangesAsync();
                }
                TempData["Message"] = "Teacher deleted successfully.";
            }
            return RedirectToAction(nameof(Teachers));
        }

        // ================= COURSE CRUD =================

        public async Task<IActionResult> Courses()
        {
            var courses = await _uow.Courses.GetAllAsync();
            var vmList = courses.Select(c => new CourseViewModel
            {
                CourseId = c.CourseId,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Department = c.Department,
                CreditHours = c.CreditHours
            }).ToList();

            return View(vmList);
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = (await _uow.Courses.FindAsync(c => c.CourseCode == model.CourseCode)).FirstOrDefault();
                if (existing != null)
                {
                    ModelState.AddModelError("CourseCode", "Course Code already exists.");
                    return View(model);
                }

                var course = new Course
                {
                    CourseCode = model.CourseCode,
                    CourseName = model.CourseName,
                    Department = model.Department,
                    CreditHours = model.CreditHours
                };

                await _uow.Courses.AddAsync(course);
                await _uow.SaveChangesAsync();

                TempData["Message"] = "Course created successfully.";
                return RedirectToAction(nameof(Courses));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _uow.Courses.GetByIdAsync(id);
            if (course == null) return NotFound();

            var vm = new CourseViewModel
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Department = course.Department,
                CreditHours = course.CreditHours
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditCourse(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = await _uow.Courses.GetByIdAsync(model.CourseId);
                if (course == null) return NotFound();

                course.CourseCode = model.CourseCode;
                course.CourseName = model.CourseName;
                course.Department = model.Department;
                course.CreditHours = model.CreditHours;

                _uow.Courses.Update(course);
                await _uow.SaveChangesAsync();

                TempData["Message"] = "Course updated successfully.";
                return RedirectToAction(nameof(Courses));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _uow.Courses.GetByIdAsync(id);
            if (course != null)
            {
                _uow.Courses.Remove(course);
                await _uow.SaveChangesAsync();
                TempData["Message"] = "Course deleted successfully.";
            }
            return RedirectToAction(nameof(Courses));
        }

        // ================= ASSIGNMENTS =================

        public async Task<IActionResult> Assignments()
        {
            var assignments = await _uow.CourseAssignments.GetAllAsync();
            var vmList = new List<CourseAssignmentViewModel>();

            foreach (var a in assignments)
            {
                var teacher = await _uow.Teachers.GetByIdAsync(a.TeacherId);
                var course = await _uow.Courses.GetByIdAsync(a.CourseId);
                vmList.Add(new CourseAssignmentViewModel
                {
                    AssignmentId = a.AssignmentId,
                    TeacherName = teacher?.FullName,
                    CourseName = course?.CourseName,
                    CourseCode = course?.CourseCode,
                    Section = a.Section,
                    Semester = a.Semester,
                    AcademicYear = a.AcademicYear,
                    Room = a.Room,
                    Schedule = a.Schedule
                });
            }
            return View(vmList);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAssignment()
        {
            var teachers = await _uow.Teachers.GetAllAsync();
            var courses = await _uow.Courses.GetAllAsync();

            var vm = new CourseAssignmentViewModel
            {
                Teachers = teachers.Select(t => new SelectListItem { Value = t.TeacherId.ToString(), Text = t.FullName }).ToList(),
                Courses = courses.Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = $"{c.CourseCode} - {c.CourseName}" }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment(CourseAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = (await _uow.CourseAssignments.FindAsync(a => 
                    a.TeacherId == model.TeacherId && 
                    a.CourseId == model.CourseId && 
                    a.Section == model.Section && 
                    a.Semester == model.Semester && 
                    a.AcademicYear == model.AcademicYear)).FirstOrDefault();

                if (existing != null)
                {
                    ModelState.AddModelError("", "This exact course assignment already exists.");
                }
                else
                {
                    // Generate legacy summary schedule string
                    string summary = "";
                    if (model.ScheduleSlots != null && model.ScheduleSlots.Any())
                    {
                        summary = string.Join(", ", model.ScheduleSlots
                            .Where(s => !string.IsNullOrEmpty(s.DayOfWeek) && !string.IsNullOrEmpty(s.StartTime) && !string.IsNullOrEmpty(s.EndTime))
                            .Select(s => $"{s.DayOfWeek.Substring(0, 3)} {s.StartTime}-{s.EndTime}"));
                    }
                    else
                    {
                        summary = model.Schedule ?? "";
                    }

                    var assignment = new CourseAssignment
                    {
                        TeacherId = model.TeacherId,
                        CourseId = model.CourseId,
                        Section = model.Section,
                        Semester = model.Semester,
                        AcademicYear = model.AcademicYear,
                        Room = model.Room,
                        Schedule = summary
                    };

                    await _uow.CourseAssignments.AddAsync(assignment);
                    await _uow.SaveChangesAsync();

                    // Save structured slots
                    if (model.ScheduleSlots != null && model.ScheduleSlots.Any())
                    {
                        foreach (var slot in model.ScheduleSlots)
                        {
                            if (!string.IsNullOrEmpty(slot.DayOfWeek) && 
                                TimeOnly.TryParse(slot.StartTime, out var startTime) && 
                                TimeOnly.TryParse(slot.EndTime, out var endTime))
                            {
                                var classSchedule = new ClassSchedule
                                {
                                    AssignmentId = assignment.AssignmentId,
                                    DayOfWeek = slot.DayOfWeek,
                                    StartTime = startTime,
                                    EndTime = endTime
                                };
                                await _uow.ClassSchedules.AddAsync(classSchedule);
                            }
                        }
                        await _uow.SaveChangesAsync();
                    }

                    TempData["Message"] = "Course Assigned successfully.";
                    return RedirectToAction(nameof(Assignments));
                }
            }

            // Re-populate dropdowns
            var teachers = await _uow.Teachers.GetAllAsync();
            var courses = await _uow.Courses.GetAllAsync();
            model.Teachers = teachers.Select(t => new SelectListItem { Value = t.TeacherId.ToString(), Text = t.FullName }).ToList();
            model.Courses = courses.Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = $"{c.CourseCode} - {c.CourseName}" }).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _uow.CourseAssignments.GetByIdAsync(id);
            if (assignment != null)
            {
                _uow.CourseAssignments.Remove(assignment);
                await _uow.SaveChangesAsync();
                TempData["Message"] = "Assignment deleted.";
            }
            return RedirectToAction(nameof(Assignments));
        }

        [HttpGet]
        public async Task<IActionResult> EditAssignment(int id)
        {
            var assignment = await _uow.CourseAssignments.GetByIdAsync(id);
            if (assignment == null) return NotFound();

            var teachers = await _uow.Teachers.GetAllAsync();
            var courses = await _uow.Courses.GetAllAsync();

            var existingSlots = await _uow.ClassSchedules.FindAsync(cs => cs.AssignmentId == assignment.AssignmentId);

            var vm = new CourseAssignmentViewModel
            {
                AssignmentId = assignment.AssignmentId,
                TeacherId = assignment.TeacherId,
                CourseId = assignment.CourseId,
                Section = assignment.Section,
                Semester = assignment.Semester,
                AcademicYear = assignment.AcademicYear,
                Room = assignment.Room,
                Schedule = assignment.Schedule,
                ScheduleSlots = existingSlots.Select(s => new ScheduleSlotInput 
                { 
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime.ToString("HH:mm"),
                    EndTime = s.EndTime.ToString("HH:mm")
                }).ToList(),
                Teachers = teachers.Select(t => new SelectListItem { Value = t.TeacherId.ToString(), Text = t.FullName }).ToList(),
                Courses = courses.Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = $"{c.CourseCode} - {c.CourseName}" }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditAssignment(CourseAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var assignment = await _uow.CourseAssignments.GetByIdAsync(model.AssignmentId);
                if (assignment == null) return NotFound();

                // Generate legacy summary schedule string
                string summary = "";
                if (model.ScheduleSlots != null && model.ScheduleSlots.Any())
                {
                    summary = string.Join(", ", model.ScheduleSlots
                        .Where(s => !string.IsNullOrEmpty(s.DayOfWeek) && !string.IsNullOrEmpty(s.StartTime) && !string.IsNullOrEmpty(s.EndTime))
                        .Select(s => $"{s.DayOfWeek.Substring(0, 3)} {s.StartTime}-{s.EndTime}"));
                }
                else
                {
                    summary = model.Schedule ?? "";
                }

                assignment.TeacherId = model.TeacherId;
                assignment.CourseId = model.CourseId;
                assignment.Section = model.Section;
                assignment.Semester = model.Semester;
                assignment.AcademicYear = model.AcademicYear;
                assignment.Room = model.Room;
                assignment.Schedule = summary;

                _uow.CourseAssignments.Update(assignment);
                await _uow.SaveChangesAsync();

                // Update slots
                var existingSlots = await _uow.ClassSchedules.FindAsync(cs => cs.AssignmentId == assignment.AssignmentId);
                foreach (var oldSlot in existingSlots)
                {
                    _uow.ClassSchedules.Remove(oldSlot);
                }
                await _uow.SaveChangesAsync();

                if (model.ScheduleSlots != null && model.ScheduleSlots.Any())
                {
                    foreach (var slot in model.ScheduleSlots)
                    {
                        if (!string.IsNullOrEmpty(slot.DayOfWeek) && 
                            TimeOnly.TryParse(slot.StartTime, out var startTime) && 
                            TimeOnly.TryParse(slot.EndTime, out var endTime))
                        {
                            var classSchedule = new ClassSchedule
                            {
                                AssignmentId = assignment.AssignmentId,
                                DayOfWeek = slot.DayOfWeek,
                                StartTime = startTime,
                                EndTime = endTime
                            };
                            await _uow.ClassSchedules.AddAsync(classSchedule);
                        }
                    }
                    await _uow.SaveChangesAsync();
                }

                TempData["Message"] = "Assignment updated successfully.";
                return RedirectToAction(nameof(Assignments));
            }

            // Re-populate dropdowns
            var teachers = await _uow.Teachers.GetAllAsync();
            var courses = await _uow.Courses.GetAllAsync();
            model.Teachers = teachers.Select(t => new SelectListItem { Value = t.TeacherId.ToString(), Text = t.FullName }).ToList();
            model.Courses = courses.Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = $"{c.CourseCode} - {c.CourseName}" }).ToList();
            return View(model);
        }

        // ================= ENROLLMENTS =================

        public async Task<IActionResult> Enrollments()
        {
            var enrollments = await _uow.Enrollments.GetAllAsync();
            var vmList = new List<EnrollmentViewModel>();

            foreach(var e in enrollments)
            {
                var student = await _uow.Students.GetByIdAsync(e.StudentId);
                var assignment = await _uow.CourseAssignments.GetByIdAsync(e.AssignmentId);
                var course = assignment != null ? await _uow.Courses.GetByIdAsync(assignment.CourseId) : null;

                vmList.Add(new EnrollmentViewModel
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentName = student?.FullName,
                    StudentCode = student?.StudentCode,
                    CourseName = course?.CourseName,
                    Section = assignment?.Section
                });
            }
            return View(vmList);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEnrollment()
        {
            var students = await _uow.Students.GetAllAsync();
            var assignments = await _uow.CourseAssignments.GetAllAsync();
            
            var assignmentList = new List<SelectListItem>();
            foreach(var a in assignments)
            {
                var course = await _uow.Courses.GetByIdAsync(a.CourseId);
                assignmentList.Add(new SelectListItem { Value = a.AssignmentId.ToString(), Text = $"{course?.CourseName} ({a.Section}) - {a.Semester}" });
            }

            var vm = new EnrollmentViewModel
            {
                Students = students.Select(s => new SelectListItem { Value = s.StudentId.ToString(), Text = $"{s.StudentCode} - {s.FullName}" }).ToList(),
                Assignments = assignmentList
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment(EnrollmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = (await _uow.Enrollments.FindAsync(e => e.StudentId == model.StudentId && e.AssignmentId == model.AssignmentId)).FirstOrDefault();
                if (existing != null)
                {
                    ModelState.AddModelError("", "Student is already enrolled in this assignment.");
                }
                else
                {
                    var enrollment = new Enrollment
                    {
                        StudentId = model.StudentId,
                        AssignmentId = model.AssignmentId
                    };
                    await _uow.Enrollments.AddAsync(enrollment);
                    await _uow.SaveChangesAsync();

                    TempData["Message"] = "Student enrolled successfully.";
                    return RedirectToAction(nameof(Enrollments));
                }
            }
            
            // Re-populate dropdowns
            var students = await _uow.Students.GetAllAsync();
            var assignments = await _uow.CourseAssignments.GetAllAsync();
            var assignmentList = new List<SelectListItem>();
            foreach(var a in assignments)
            {
                var course = await _uow.Courses.GetByIdAsync(a.CourseId);
                assignmentList.Add(new SelectListItem { Value = a.AssignmentId.ToString(), Text = $"{course?.CourseName} ({a.Section}) - {a.Semester}" });
            }
            model.Students = students.Select(s => new SelectListItem { Value = s.StudentId.ToString(), Text = $"{s.StudentCode} - {s.FullName}" }).ToList();
            model.Assignments = assignmentList;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var e = await _uow.Enrollments.GetByIdAsync(id);
            if (e != null)
            {
                _uow.Enrollments.Remove(e);
                await _uow.SaveChangesAsync();
                TempData["Message"] = "Enrollment removed.";
            }
            return RedirectToAction(nameof(Enrollments));
        }

        [HttpGet]
        public async Task<IActionResult> EditEnrollment(int id)
        {
            var e = await _uow.Enrollments.GetByIdAsync(id);
            if (e == null) return NotFound();

            var students = await _uow.Students.GetAllAsync();
            var assignments = await _uow.CourseAssignments.GetAllAsync();
            var assignmentList = new List<SelectListItem>();
            foreach (var a in assignments)
            {
                var course = await _uow.Courses.GetByIdAsync(a.CourseId);
                assignmentList.Add(new SelectListItem { Value = a.AssignmentId.ToString(), Text = $"{course?.CourseName} ({a.Section}) - {a.Semester}" });
            }

            var vm = new EnrollmentViewModel
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                AssignmentId = e.AssignmentId,
                Students = students.Select(s => new SelectListItem { Value = s.StudentId.ToString(), Text = $"{s.StudentCode} - {s.FullName}" }).ToList(),
                Assignments = assignmentList
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditEnrollment(EnrollmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var e = await _uow.Enrollments.GetByIdAsync(model.EnrollmentId);
                if (e == null) return NotFound();

                // Check if this student is already enrolled in this assignment (excluding current record)
                var existing = (await _uow.Enrollments.FindAsync(x => x.StudentId == model.StudentId && x.AssignmentId == model.AssignmentId && x.EnrollmentId != model.EnrollmentId)).FirstOrDefault();
                if (existing != null)
                {
                    ModelState.AddModelError("", "Student is already enrolled in this assignment.");
                }
                else
                {
                    e.StudentId = model.StudentId;
                    e.AssignmentId = model.AssignmentId;

                    _uow.Enrollments.Update(e);
                    await _uow.SaveChangesAsync();

                    TempData["Message"] = "Enrollment updated successfully.";
                    return RedirectToAction(nameof(Enrollments));
                }
            }

            // Re-populate dropdowns
            var students = await _uow.Students.GetAllAsync();
            var assignments = await _uow.CourseAssignments.GetAllAsync();
            var assignmentList = new List<SelectListItem>();
            foreach (var a in assignments)
            {
                var course = await _uow.Courses.GetByIdAsync(a.CourseId);
                assignmentList.Add(new SelectListItem { Value = a.AssignmentId.ToString(), Text = $"{course?.CourseName} ({a.Section}) - {a.Semester}" });
            }
            model.Students = students.Select(s => new SelectListItem { Value = s.StudentId.ToString(), Text = $"{s.StudentCode} - {s.FullName}" }).ToList();
            model.Assignments = assignmentList;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportAttendance(int assignmentId)
        {
            var fileBytes = await _reportService.ExportAttendanceReportAsync(assignmentId, "excel");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
        }

        // ================= ADVANCED ADMIN FEATURES =================

        public async Task<IActionResult> ActivityLogs()
        {
            var logs = await _uow.ActivityLogs.GetAllAsync();
            var vmList = new List<ActivityLogViewModel>();

            foreach(var log in logs.OrderByDescending(l => l.Timestamp).Take(100))
            {
                var user = await _uow.Users.GetByIdAsync(log.UserId);
                vmList.Add(new ActivityLogViewModel
                {
                    LogId = log.LogId,
                    Username = user?.Username ?? "Unknown",
                    Action = log.Action,
                    Description = log.Description,
                    Timestamp = log.Timestamp
                });
            }
            return View(vmList);
        }

        [HttpGet]
        public IActionResult BulkUploadStudents()
        {
            return View(new BulkUploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> BulkUploadStudents(BulkUploadViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a valid CSV file.");
                return View(model);
            }

            int success = 0, fail = 0;
            using (var reader = new StreamReader(model.File.OpenReadStream()))
            {
                var headers = await reader.ReadLineAsync(); // skip header
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',');
                    if (values.Length >= 8)
                    {
                        try
                        {
                            var user = new User
                            {
                                Username = values[0].Trim(),
                                Email = values[1].Trim(),
                                PasswordHash = BCrypt.Net.BCrypt.HashPassword(values[2].Trim()),
                                Role = "Student"
                            };
                            await _uow.Users.AddAsync(user);
                            await _uow.SaveChangesAsync();

                            var student = new Student
                            {
                                UserId = user.UserId,
                                StudentCode = values[3].Trim(),
                                FullName = values[4].Trim(),
                                Department = values[5].Trim(),
                                Batch = values[6].Trim(),
                                Section = values[7].Trim(),
                                ContactNumber = values.Length > 8 ? values[8].Trim() : ""
                            };
                            await _uow.Students.AddAsync(student);
                            await _uow.SaveChangesAsync();
                            success++;
                        }
                        catch
                        {
                            fail++;
                        }
                    }
                }
            }

            model.SuccessCount = success;
            model.FailCount = fail;
            model.Message = $"Upload complete. Success: {success}, Failed: {fail}";
            return View(model);
        }
    }
}
