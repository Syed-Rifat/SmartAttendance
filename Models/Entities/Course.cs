using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(20)]
        public string CourseCode { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string CourseName { get; set; } = "";

        [Required]
        [MaxLength(60)]
        public string Department { get; set; } = "";

        public int CreditHours { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
