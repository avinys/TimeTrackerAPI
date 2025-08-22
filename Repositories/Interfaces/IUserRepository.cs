using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User? GetById(int id);
        User? GetByUsername(string username);
        User? GetByEmail(string email);
        void Add(User user);
        void Save();
    }
}
