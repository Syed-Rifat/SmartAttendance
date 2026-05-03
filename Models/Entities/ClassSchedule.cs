using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class ClassSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public int AssignmentId { get; set; }
        public CourseAssignment? CourseAssignment { get; set; }

        [Required]
        [MaxLength(10)]
        public string DayOfWeek { get; set; } = "";  // "Sunday", "Monday", etc.

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }
    }
}
