using System.Collections.Generic;

namespace SmartAttendance.Models.ViewModels
{
    public class TeacherDashboardViewModel
    {
        public string TeacherName { get; set; } = "";
        public string Department { get; set; } = "";
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public List<TeacherCourseItem> Courses { get; set; } = new();
    }

    public class TeacherCourseItem
    {
        public int AssignmentId { get; set; }
        public string CourseName { get; set; } = "";
        public string CourseCode { get; set; } = "";
        public string Section { get; set; } = "";
        public string Semester { get; set; } = "";
        public string? Room { get; set; }
        public string? Schedule { get; set; }
        public int EnrolledStudentCount { get; set; }
    }
}
