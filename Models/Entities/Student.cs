using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(20)]
        public string StudentCode { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [Required]
        [MaxLength(60)]
        public string Department { get; set; } = "";

        [Required]
        [MaxLength(10)]
        public string Batch { get; set; } = "";

        [Required]
        [MaxLength(5)]
        public string Section { get; set; } = "";

        [MaxLength(15)]
        public string? ContactNumber { get; set; }
    }
}
