using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
