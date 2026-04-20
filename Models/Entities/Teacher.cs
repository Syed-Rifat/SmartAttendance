using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [Required]
        [MaxLength(60)]
        public string Department { get; set; } = "";

        [MaxLength(15)]
        public string? ContactNumber { get; set; }
    }
}
