using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackerAPI.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        // Foreign key → User
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;  // Navigation property

        // SHA-256 hex string (64 chars) or binary(32)
        [Required]
        [MaxLength(64)]
        public string TokenHash { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? ConsumedAtUtc { get; set; }

        [Required]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(45)]
        public string? CreatedIp { get; set; }

        [MaxLength(255)]
        public string? CreatedUserAgent { get; set; }

        [MaxLength(32)]
        public string Purpose { get; set; } = "password-reset";
    }
}
