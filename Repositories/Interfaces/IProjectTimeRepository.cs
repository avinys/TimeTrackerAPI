using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IProjectTimeRepository
    {
        IQueryable<ProjectTime> Query(); // Base for Scoped()
        Task<ProjectTime?> GetByIdAsync(int id);
        Task AddAsync(ProjectTime projectTIme);
        void Remove(ProjectTime projectTIme);
        Task SaveAsync();
    }
}