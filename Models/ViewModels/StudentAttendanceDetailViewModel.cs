using System;
using System.Collections.Generic;

namespace SmartAttendance.Models.ViewModels
{
    public class StudentAttendanceDetailViewModel
    {
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public List<CourseAttendanceInfo> Courses { get; set; } = new List<CourseAttendanceInfo>();
    }

    public class CourseAttendanceInfo
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Section { get; set; }
        public string Semester { get; set; }
        public int TotalClasses { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public decimal AttendancePercentage { get; set; }
        public bool IsLowAttendance { get; set; }
        public List<AttendanceRecordDetail> Records { get; set; } = new List<AttendanceRecordDetail>();
    }

    public class AttendanceRecordDetail
    {
        public DateOnly Date { get; set; }
        public string Status { get; set; }
    }

    public class TeacherAttendanceSummaryViewModel
    {
        public int AssignmentId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Section { get; set; }
        public string Semester { get; set; }
        public List<StudentSummaryItem> Students { get; set; } = new List<StudentSummaryItem>();
    }

    public class StudentSummaryItem
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public int TotalClasses { get; set; }
        public int Attended { get; set; }
        public decimal Percentage { get; set; }
        public bool IsLow { get; set; }
    }
}
