using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = "";
        [Required]
        public string NewPassword { get; set; } = "";
    }
}
