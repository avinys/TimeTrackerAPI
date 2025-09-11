using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public Task<IEnumerable<User>> GetUsersAsync() =>
            _repo.GetUsersAsync().ContinueWith(t => (IEnumerable<User>)t.Result);

        public Task<User?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<User?> GetByUsernameAsync(string username) => _repo.GetByUsernameAsync(username);
        public Task<User?> GetByEmailAsync(string email) => _repo.GetByEmailAsync(email);

        public async Task<User> CreateUserAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new Exception("Password is required");

            // Uniqueness checks (use AnyAsync via Query for efficiency)
            if (await _repo.Query().AnyAsync(u => u.Username == username))
                throw new Exception("Username already exists");
            if (await _repo.Query().AnyAsync(u => u.Email == email))
                throw new Exception("Email already exists");

            var hash = BCrypt.Net.BCrypt.HashPassword(password); // CPU-bound, keep sync
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash,
                Role = "User"
            };

            await _repo.AddAsync(user);
            await _repo.SaveAsync();
            return user;
        }

        public async Task<User?> ValidateLoginAsync(string identifier, string password)
        {
            User? user = identifier.Contains('@')
                ? await _repo.GetByEmailAsync(identifier)
                : await _repo.GetByUsernameAsync(identifier);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash)) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

            return user;
        }

        public async Task<User> CreateOrLinkExternalUserAsync(string provider, string providerUserId, string email, string? fullName)
        {
            var byProvider = await _repo.GetByProviderAsync(provider, providerUserId);
            if (byProvider != null) return byProvider;

            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Username = email, // or a generated username
                    Email = email,
                    PasswordHash = null,
                    Role = "User"
                };
                await _repo.AddAsync(user);
                await _repo.SaveAsync();
            }

            await _repo.AddProviderLinkAsync(new UserIdentityProvider
            {
                UserId = user.Id,
                Provider = provider,
                ProviderUserId = providerUserId,
                ProviderEmail = email
            });
            await _repo.SaveAsync();

            return user;
        }
        public async Task SaveAsync(User user)
        {
            _repo.Update(user);
            await _repo.SaveAsync();
        }
    }
}