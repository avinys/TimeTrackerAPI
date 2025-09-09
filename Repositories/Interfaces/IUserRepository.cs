using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User? GetById(int id);
        User? GetByUsername(string username);
        User? GetByEmail(string email);

        // Provider support
        User? GetByProvider(string provider, string providerUserId);
        void AddProviderLink(UserIdentityProvider link);

        void Add(User user);
        void Save();
    }
}
