using System.ComponentModel.DataAnnotations;

namespace TimeTrackerAPI.Models
{
    public class User
    {
        public int Id {  get; set; }
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "User";
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        
        public string? PasswordHash { get; set; }

        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<UserIdentityProvider> ExternalLogins { get; set; } = new List<UserIdentityProvider>();

    }
}
