using System;
using System.ComponentModel.DataAnnotations;

namespace SmartAttendance.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = "";

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
