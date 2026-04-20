using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.ViewModels
{
    public class TeacherViewModel
    {
        public int TeacherId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Department { get; set; }

        public string ContactNumber { get; set; }

        // For creating a new teacher (also creates User account)
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class TeacherEditViewModel
    {
        public int TeacherId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Department { get; set; }

        public string ContactNumber { get; set; }
    }

    public class TeacherListViewModel
    {
        public int TeacherId { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string ContactNumber { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
