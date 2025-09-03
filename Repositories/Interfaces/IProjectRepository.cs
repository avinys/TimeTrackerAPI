using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        IQueryable<Project> Query();
        Task<Project?> GetByIdAsync(int id);
        Task AddAsync(Project project);
        void Remove(Project project);
        Task SaveAsync();
    }
}