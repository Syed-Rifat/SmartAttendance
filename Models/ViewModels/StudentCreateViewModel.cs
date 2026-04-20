using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.ViewModels
{
    public class StudentCreateViewModel
    {
        // User account
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Student info
        [Required]
        public string StudentCode { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Batch { get; set; }

        [Required]
        public string Section { get; set; }

        public string ContactNumber { get; set; }
    }

    public class StudentEditViewModel
    {
        public int StudentId { get; set; }

        [Required]
        public string StudentCode { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Batch { get; set; }

        [Required]
        public string Section { get; set; }

        public string ContactNumber { get; set; }
    }
}
