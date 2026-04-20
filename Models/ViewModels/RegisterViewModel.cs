using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Additional fields for student registration
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
    }
}
