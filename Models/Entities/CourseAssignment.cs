using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class CourseAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required]
        [MaxLength(5)]
        public string Section { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string Semester { get; set; } = "";

        [Required]
        [MaxLength(10)]
        public string AcademicYear { get; set; } = "";

        [MaxLength(20)]
        public string? Room { get; set; }

        [MaxLength(100)]
        public string? Schedule { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
