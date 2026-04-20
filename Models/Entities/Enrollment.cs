using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int AssignmentId { get; set; }
        public CourseAssignment? CourseAssignment { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
