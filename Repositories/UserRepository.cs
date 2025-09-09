using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Data;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;

namespace TimeTrackerAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) => _context = context;

        public IQueryable<User> Query() => _context.Users.AsQueryable();

        public Task<List<User>> GetUsersAsync() =>
            _context.Users.AsNoTracking().ToListAsync();

        public Task<User?> GetByIdAsync(int id) =>
            _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public Task<User?> GetByUsernameAsync(string username) =>
            _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public Task<User?> GetByEmailAsync(string email) =>
            _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByProviderAsync(string provider, string providerUserId)
        {
            return await _context.UserIdentityProviders
                .Include(x => x.User)
                .Where(x => x.Provider == provider && x.ProviderUserId == providerUserId)
                .Select(x => x.User)
                .FirstOrDefaultAsync();
        }

        public Task AddAsync(User user) =>
            _context.Users.AddAsync(user).AsTask();

        public Task AddProviderLinkAsync(UserIdentityProvider link) =>
            _context.UserIdentityProviders.AddAsync(link).AsTask();

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
