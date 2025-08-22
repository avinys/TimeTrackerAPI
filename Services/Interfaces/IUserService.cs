using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers();
        User? GetById(int id);
        User? GetByUsername(string username); // Is really needed??
        User? GetByEmail(string email); // Is really needed??
        User? ValidateLogin(string identifier, string password);
        User CreateUser(string username, string email, string password);
    }
}
