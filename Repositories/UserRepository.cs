using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Data;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;

namespace TimeTrackerAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }
        public User? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetByProvider(string provider, string providerUserId)
        {
            return _context.UserIdentityProviders
                .Include(x => x.User)
                .FirstOrDefault(x => x.Provider == provider && x.ProviderUserId == providerUserId)
                ?.User;
        }

        public void AddProviderLink(UserIdentityProvider link)
        {
            _context.UserIdentityProviders.Add(link);
        }
        public void Add(User user)
        {
            _context.Users.Add(user);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
