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
