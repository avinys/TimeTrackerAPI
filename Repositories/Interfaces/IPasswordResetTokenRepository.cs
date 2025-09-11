using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetAsync(int userId, string tokenHash, string purpose);
        Task InvalidateOthersAsync(int userId, int exceptId, DateTime consumedAtUtc);
        Task SaveAsync();
    }
}
