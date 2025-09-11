using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Data;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;

namespace TimeTrackerAPI.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public PasswordResetTokenRepository(ApplicationDbContext context) => _context = context;

        public Task AddAsync(PasswordResetToken token) =>
            _context.PasswordResetTokens.AddAsync(token).AsTask();

        public Task<PasswordResetToken?> GetAsync(int userId, string tokenHash, string purpose) =>
            _context.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId &&
                    t.Purpose == purpose &&
                    t.TokenHash == tokenHash);

        public async Task InvalidateOthersAsync(int userId, int exceptId, DateTime consumedAtUtc)
        {
            var others = _context.PasswordResetTokens
                .Where(t => t.UserId == userId && t.ConsumedAtUtc == null && t.Id != exceptId);

            await others.ForEachAsync(t => t.ConsumedAtUtc = consumedAtUtc);
        }

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
