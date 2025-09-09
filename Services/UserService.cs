using BCrypt.Net;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<User> GetUsers()
        {
            return _repo.GetUsers();
        }
        public User? GetById(int id)
        {
            return _repo.GetById(id);
        }
        public User? GetByUsername(string username)
        {
            return _repo.GetByUsername(username);
        }
        public User? GetByEmail(string email)
        {
            return _repo.GetByEmail(email);
        }
        public User CreateUser(string username, string email, string password)
        {
            if (_repo.GetByUsername(username) != null)
            {
                throw new Exception("Username already exists");
            }
            if (_repo.GetByEmail(email) != null)
            {
                throw new Exception("Email already exists");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Password is required");
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash,
                Role = "User"
            };
            _repo.Add(user);
            _repo.Save();
            return user;
        }
        public User? ValidateLogin(string identifier, string password)
        {
            User? user = identifier.Contains('@')
                ? _repo.GetByEmail(identifier)
                : _repo.GetByUsername(identifier);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }
        public User CreateOrLinkExternalUser(string provider, string providerUserId, string email, string? fullName)
        {
            var byProvider = _repo.GetByProvider(provider, providerUserId);
            if(byProvider != null)
            {
                return byProvider;
            }

            var user = GetByEmail(email);
            if(user == null)
            {
                user = new User
                {
                    Username = email,
                    Email = email,
                    PasswordHash = null,
                    Role = "User"
                };
                _repo.Add(user);
                _repo.Save();
            }

            _repo.AddProviderLink(new UserIdentityProvider
            {
                UserId = user.Id,
                Provider = provider,
                ProviderUserId = providerUserId,
                ProviderEmail = email

            });

            _repo.Save();
            return user;

        }
    }
}