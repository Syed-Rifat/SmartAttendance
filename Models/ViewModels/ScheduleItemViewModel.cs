namespace SmartAttendance.Models.ViewModels
{
    public class ScheduleItemViewModel
    {
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Room { get; set; }
        public string? Schedule { get; set; }
        public string? TeacherName { get; set; }

        // Structured schedule fields from ClassSchedules table
        public string? DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
