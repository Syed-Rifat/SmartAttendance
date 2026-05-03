using System.Collections.Generic;

namespace SmartAttendance.Models.ViewModels
{
    public class TeacherOverviewViewModel
    {
        public string TeacherName { get; set; } = "";
        public string Department { get; set; } = "";
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        
        // Classes scheduled for today
        public List<ScheduleItemViewModel> TodayClasses { get; set; } = new List<ScheduleItemViewModel>();
    }
}
