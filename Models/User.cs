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

        // Email confirmation
        public bool EmailConfirmed { get; set; } = false;
        public string? EmailConfirmationTokenHash { get; set; }
        public DateTime? EmailConfirmationExpiresAt { get; set; }

        // Resend limiting
        public DateTime? EmailConfirmationLastSentAt { get; set; }
        public DateTime? EmailConfirmationResendCountWindowStart { get; set; }
        public int EmailConfirmationResendCount { get; set; }

        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    }
}
