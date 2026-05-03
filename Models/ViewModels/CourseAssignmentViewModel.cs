using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartAttendance.Models.ViewModels
{
    public class CourseAssignmentViewModel
    {
        public int AssignmentId { get; set; }

        [Required(ErrorMessage = "Please select a teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Please select a course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Section is required")]
        public string Section { get; set; } = "";

        [Required(ErrorMessage = "Semester is required")]
        public string Semester { get; set; } = "";

        [Required(ErrorMessage = "Academic year is required")]
        public string AcademicYear { get; set; } = "";

        public string? Room { get; set; }
        public string? Schedule { get; set; }

        // Schedule slots for multi-day/time entry
        public List<ScheduleSlotInput> ScheduleSlots { get; set; } = new List<ScheduleSlotInput>();

        // For dropdowns — not validated
        public List<SelectListItem>? Teachers { get; set; }
        public List<SelectListItem>? Courses { get; set; }

        // For display only — not validated
        public string? TeacherName { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
    }

    public class ScheduleSlotInput
    {
        public string DayOfWeek { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
    }
}
