using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartAttendance.Models.ViewModels
{
    public class EnrollmentViewModel
    {
        public int EnrollmentId { get; set; }

        [Required(ErrorMessage = "Please select a student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Please select a course assignment")]
        public int AssignmentId { get; set; }

        // For dropdowns — not validated
        public List<SelectListItem>? Students { get; set; }
        public List<SelectListItem>? Assignments { get; set; }

        // For display only — not validated
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public string? CourseName { get; set; }
        public string? Section { get; set; }
    }
}
