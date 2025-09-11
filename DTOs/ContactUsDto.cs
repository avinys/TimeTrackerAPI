using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class ContactUsDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = "";

        [MaxLength(120)]
        public string? Subject { get; set; }

        [Required, MaxLength(4000)]
        public string Message { get; set; } = "";

        // honeypot (should be empty)
        public string? Website { get; set; }
        public string? Phone { get; set; }
    }
}
