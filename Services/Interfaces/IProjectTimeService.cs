using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IProjectTimeService
    {
        public IEnumerable<ProjectTime> GetProjectTimes();
        public ProjectTime? GetById(int id);
        public IEnumerable<ProjectTime> GetByUserAndProjectId(int userId, int projectId);
        public ProjectTime Create(int userId, int projectId);
        public ProjectTime Update(int projectTimeId, int userId, DateTime startTime, DateTime endTime);
        public void Delete(int id, int userId);
    }
}
