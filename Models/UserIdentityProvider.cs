namespace TimeTrackerAPI.Models
{
    public class UserIdentityProvider
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Provider { get; set; } = default!;       // "google"
        public string ProviderUserId { get; set; } = default!; // Google "sub"
        public string? ProviderEmail { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = default!;
    }
}
