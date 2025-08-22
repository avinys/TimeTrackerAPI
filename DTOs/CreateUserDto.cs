using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[^@]+$", ErrorMessage = "Username cannot contain '@'.")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }
    }
}
