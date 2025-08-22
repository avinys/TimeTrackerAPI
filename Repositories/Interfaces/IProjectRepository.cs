using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        IEnumerable<Project> GetProjects();
        IEnumerable<Project> GetByUserId(int userId);
        Project? GetById(int id);
        void Add(Project project);
        void Save();
    }
}
