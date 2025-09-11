using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
