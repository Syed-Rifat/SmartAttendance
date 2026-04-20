using System;

namespace SmartAttendance.Models.ViewModels
{
    public class ActivityLogViewModel
    {
        public int LogId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
