using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.ViewModels
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }

        [Required]
        public string CourseCode { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        [Range(1, 10)]
        public int CreditHours { get; set; }
    }
}
