using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IProjectTimeRepository
    {
        IEnumerable<ProjectTime> GetProjectTimes();
        ProjectTime? GetById(int id);
        IEnumerable<ProjectTime> GetByUserAndProjectId(int userId, int projectId);
        void Add(ProjectTime projectTime);
        void Update(ProjectTime projectTime);
        void Delete(int id);
        void Save();
    }
}