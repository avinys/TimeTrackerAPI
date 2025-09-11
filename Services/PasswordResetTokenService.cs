using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public class PasswordResetTokenService : IPasswordResetTokenService
    {
        private readonly IPasswordResetTokenRepository _repo;

        public PasswordResetTokenService(IPasswordResetTokenRepository repo) => _repo = repo;

        public async Task<PasswordResetToken> CreateAsync(
            int userId,
            string tokenHash,
            DateTime expiresAtUtc,
            string? ip,
            string? userAgent,
            string purpose = "password-reset")
        {
            var entity = new PasswordResetToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAtUtc = expiresAtUtc,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedIp = ip,
                CreatedUserAgent = userAgent,
                Purpose = purpose
            };

            await _repo.AddAsync(entity);
            await _repo.SaveAsync();
            return entity;
        }

        public async Task<PasswordResetToken?> GetValidAsync(int userId, string tokenHash, string purpose = "password-reset")
        {
            var prt = await _repo.GetAsync(userId, tokenHash, purpose);
            if (prt is null) return null;
            if (prt.ConsumedAtUtc != null) return null;
            if (prt.ExpiresAtUtc < DateTime.UtcNow) return null;
            return prt;
        }

        public async Task ConsumeAsync(PasswordResetToken token)
        {
            token.ConsumedAtUtc = DateTime.UtcNow;
            await _repo.SaveAsync();
        }

        public Task InvalidateOthersAsync(int userId, int exceptId) =>
            _repo.InvalidateOthersAsync(userId, exceptId, DateTime.UtcNow);
    }
}
