using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username / Email / Student Code is required")]
        [Display(Name = "Username / Email / Student Code")]
        public string LoginId { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please select your role")]
        [Display(Name = "Login As")]
        public string Role { get; set; } = "";
    }
}
