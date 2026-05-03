using System.Collections.Generic;

namespace SmartAttendance.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = "";
        public string StudentCode { get; set; } = "";
        public string Department { get; set; } = "";
        public string Batch { get; set; } = "";
        public int TotalEnrolledCourses { get; set; }
        public decimal OverallAttendancePercentage { get; set; }
        public int LowAttendanceCourseCount { get; set; }
        public int TotalClassesAttended { get; set; }
        public int TotalClassesConducted { get; set; }
        public List<StudentCourseStatItem> CourseStats { get; set; } = new();
        public List<ScheduleItemViewModel> TodayClasses { get; set; } = new();
    }

    public class StudentCourseStatItem
    {
        public string CourseName { get; set; } = "";
        public string CourseCode { get; set; } = "";
        public string Section { get; set; } = "";
        public string Semester { get; set; } = "";
        public decimal AttendancePercentage { get; set; }
        public int TotalClasses { get; set; }
        public int PresentCount { get; set; }
        public bool IsLow { get; set; }
    }
}
