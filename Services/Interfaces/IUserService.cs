using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateUserAsync(string username, string email, string password);
        Task<User?> ValidateLoginAsync(string identifier, string password);
        Task<User> CreateOrLinkExternalUserAsync(string provider, string providerUserId, string email, string? fullName);
        Task SaveAsync(User user);
    }
}
