using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Exceptions;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public class ProjectTimeService : IProjectTimeService
    {
        private readonly IProjectTimeRepository _repo;
        private readonly IProjectService _projectService;
        private readonly ICurrentUserService _current;

        public ProjectTimeService(IProjectTimeRepository repository, IProjectService projectService, ICurrentUserService current)
        {
            _repo = repository;
            _projectService = projectService;
            _current = current;
        }

        private int RequireUser() => _current.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");
        private IQueryable<ProjectTime> Scoped()
        {
            if (_current.IsAdmin) return _repo.Query();
            var userId = RequireUser();
            return _repo.Query().Where(pt => pt.UserId == userId);
        }

        public async Task<IEnumerable<ProjectTime>> GetProjectTimesAsync()
        {
            return await Scoped().AsNoTracking().ToListAsync();
        }

        public async Task<ProjectTime?> GetByIdAsync(int id)
        {
            return await Scoped().AsNoTracking().FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task<IEnumerable<ProjectTime>> GetByUserAndProjectIdAsync(int userId, int projectId)
        {
            // Let admin access all users project time lists and for non-admin users, only their own
            var currentUserId = RequireUser();
            if(!_current.IsAdmin && currentUserId != userId)
                throw new UnauthorizedAccessException("Cannot access another user's project times");

            IQueryable<ProjectTime> query = Scoped().Where(pt => pt.ProjectId == projectId);

            if (_current.IsAdmin)
                query = query.Where(pt => pt.UserId == userId);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<ProjectTime> CreateAsync(int userId, int projectId)
        {
            var currentUserId = RequireUser();
            if(!_current.IsAdmin && currentUserId != userId)
                throw new UnauthorizedAccessException("Cannot create entry for another user");

            var project = await _projectService.GetByIdAsync(projectId) ?? throw new NotFoundException("Project not found");

            if (!_current.IsAdmin && project.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this project");

            var hasOpen = await _repo.Query()
                   .AnyAsync(pt => pt.UserId == userId && pt.ProjectId == projectId && pt.EndTime == null);

            if (hasOpen)
                throw new ValidationException("You already have an ongoing timer. Stop it before starting a new one.");

            var projectTime = new ProjectTime
            {
                UserId = userId,
                ProjectId = projectId,
                StartTime = DateTime.UtcNow,
                EndTime = null,
                Comment = ""
            };
            await _repo.AddAsync(projectTime);
            await _repo.SaveAsync();
            return projectTime;
        }

        public async Task<ProjectTime> UpdateAsync(int projectTimeId, DateTimeOffset? startTime, DateTimeOffset? endTime, string? comment)
        {
            var projectTime = await Scoped().FirstOrDefaultAsync(pt => pt.Id == projectTimeId) ?? 
                throw new NotFoundException("Project time not found.");

            DateTime? startUtc = startTime?.UtcDateTime;
            DateTime? endUtc = endTime?.UtcDateTime;
            DateTime nowUtc = DateTime.UtcNow;

            // Start cannot be in the future (more applicable, when is ongoing - endTime is null)
            if (startTime > nowUtc)
                throw new ValidationException("Start time cannot be in the future");

            // Fetch adjacent entries for the same user and project
            var q = _repo.Query()
                .Where(pt =>
                pt.UserId == projectTime.UserId &&
                pt.Id != projectTime.Id &&
                pt.ProjectId == projectTime.ProjectId);

            var currentStartTime = projectTime.StartTime;

            var previous = await q.Where(pt => pt.StartTime < currentStartTime)
                .OrderByDescending(pt => pt.StartTime)
                .FirstOrDefaultAsync();
            var next = await q.Where(pt => pt.StartTime > currentStartTime)
                .OrderBy(pt => pt.StartTime)
                .FirstOrDefaultAsync();

            // Start must be later than endTime of previous entry
            if (previous != null)
            {
                if (previous.EndTime == null)
                    throw new ValidationException("Invariant violated: previous entry must be closed.");
                if(startUtc <= previous.EndTime)
                    throw new ValidationException("Start time must be after the previous entry");
            }

            if(endUtc.HasValue)
            {
                // End must be later than start
                if (endUtc.Value <= startUtc)
                    throw new ValidationException("End time must be later than startTime");

                // End must be earlier than now
                if (endUtc.Value > nowUtc)
                    throw new ValidationException("End time cannot be in the future");

                // End must be earlier than the next entry's start
                if (next != null && endUtc.Value >= next.StartTime)
                    throw new ValidationException("End time must be earlier than the next entry");
            }
            else
            {
                // Project time can be ongoing only if there is no next entry already
                if (next != null)
                    throw new ValidationException("Project time can be ongoing only when there exists no next entry");
            }

            if (startUtc.HasValue) projectTime.StartTime = startUtc.Value;
            if (endTime is not null) projectTime.EndTime = endUtc;
            if (comment != null) projectTime.Comment = comment;

            await _repo.SaveAsync();
            return projectTime;
        }

        public async Task DeleteAsync(int id)
        {
            var projectTime = await Scoped().FirstOrDefaultAsync(pt => pt.Id == id) ?? 
                throw new NotFoundException("Project time not found.");

            _repo.Remove(projectTime);
            await _repo.SaveAsync();
        }
    }
}
