using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceId { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public int AssignmentId { get; set; }
        public CourseAssignment? CourseAssignment { get; set; }

        [Required]
        public DateOnly AttendanceDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string Status { get; set; } = "Present";

        public DateTime MarkedAt { get; set; } = DateTime.UtcNow;

        public int MarkedByTeacherId { get; set; }
        public Teacher? MarkedByTeacher { get; set; }

        public DateTime? EditedAt { get; set; }
    }
}
