using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TimeTrackerAPI.Models;

namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IProjectTimeService
    {
        Task<IEnumerable<ProjectTime>> GetProjectTimesAsync();
        Task<ProjectTime?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectTime>> GetByUserAndProjectIdAsync(int userId, int projectId);
        Task<ProjectTime> CreateAsync(int userId, int projectId);
        Task<ProjectTime> UpdateAsync(int projectTimeId, DateTimeOffset? startUtc, DateTimeOffset? endUtc, string? comment);
        Task DeleteAsync(int id);
    }
}
