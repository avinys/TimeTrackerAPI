using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IPasswordResetTokenService
    {
        Task<PasswordResetToken> CreateAsync(
            int userId,
            string tokenHash,
            DateTime expiresAtUtc,
            string? ip,
            string? userAgent,
            string purpose = "password-reset");

        Task<PasswordResetToken?> GetValidAsync(int userId, string tokenHash, string purpose = "password-reset");

        Task ConsumeAsync(PasswordResetToken token);

        Task InvalidateOthersAsync(int userId, int exceptId);
    }
}
