using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> Query();
        Task<List<User>> GetUsersAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByProviderAsync(string provider, string providerUserId);
        Task AddAsync(User user);
        Task AddProviderLinkAsync(UserIdentityProvider link);
        void Update(User user);
        Task SaveAsync();
    }
}
