using System;
using System.Collections.Generic;

namespace SmartAttendance.Models.ViewModels
{
    public class AttendanceViewModel
    {
        public int AssignmentId { get; set; }
        public string CourseName { get; set; }
        public string Section { get; set; }
        public DateOnly AttendanceDate { get; set; }
        
        public List<StudentAttendanceItem> Students { get; set; } = new List<StudentAttendanceItem>();
    }

    public class StudentAttendanceItem
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; } // Present, Absent, Late
    }
}
