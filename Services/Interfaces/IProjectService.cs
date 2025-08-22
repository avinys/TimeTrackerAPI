using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<Project> GetProjects();
        IEnumerable<Project> GetByUserId(int userId);
        Project? GetById(int id);
        Project Create(string name, int userId);
    }
}

