using TimeTrackerAPI.Models;
using TimeTrackerAPI.DTOs;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjectsAsync();
        Task<ProjectDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectDto>> GetByUserIdAsync(int userId);
        Task<ProjectDto> CreateAsync(string name, int userId);
        Task<ProjectDto> UpdateAsync(int projectId, UpdateProjectDto dto);
        Task DeleteAsync(int id);
    }
}

