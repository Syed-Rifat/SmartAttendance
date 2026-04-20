using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.ViewModels
{
    public class BulkUploadViewModel
    {
        [Required]
        public IFormFile File { get; set; }

        public string Message { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
    }
}
