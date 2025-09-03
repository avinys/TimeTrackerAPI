using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Exceptions;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repo;
        private readonly IProjectTimeRepository _repoTimes;
        private readonly ICurrentUserService _current;
        public ProjectService(IProjectRepository repo, ICurrentUserService current, IProjectTimeRepository repoTimes)
        {
            _repo = repo;
            _current = current;
            _repoTimes = repoTimes;
        }
        private int RequireUser() => _current.UserId ?? throw new UnauthorizedAccessException("User not authenticated.");
        private IQueryable<Project> Scoped()
        {
            if (_current.IsAdmin) return _repo.Query();
            var userId = RequireUser();
            return _repo.Query().Where(p => p.UserId == userId);
        }
        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
        {
            return await Scoped()
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Name = p.Name,
                    Correction = p.Correction,
                    // entity CreatedAt assumed UTC DateTime
                    CreatedAt = new DateTimeOffset(p.CreatedAt, TimeSpan.Zero),
                    IsRunning = _repoTimes.Query().Any(t =>
                                   t.ProjectId == p.Id &&
                                   t.UserId == p.UserId &&
                                   t.EndTime == null)
                })
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<ProjectDto?> GetByIdAsync(int id)
        {
            return await Scoped()
                .Where(p => p.Id == id)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Name = p.Name,
                    Correction = p.Correction,
                    CreatedAt = new DateTimeOffset(p.CreatedAt, TimeSpan.Zero),
                    IsRunning = _repoTimes.Query().Any(t =>
                                     t.UserId == p.UserId &&
                                     t.ProjectId == p.Id &&
                                     t.EndTime == null)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<ProjectDto>> GetByUserIdAsync(int userId)
        {
            // Let admin access all users project time lists and for non-admin users, only their own
            var currentUserId = RequireUser();
            if (!_current.IsAdmin && currentUserId != userId)
                throw new UnauthorizedAccessException("Cannot access another user's projects");

            return await _repo.Query()
                .Where(p => p.UserId == userId)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Name = p.Name,
                    Correction = p.Correction,
                    CreatedAt = new DateTimeOffset(p.CreatedAt, TimeSpan.Zero),
                    IsRunning = _repoTimes.Query().Any(t =>
                                   t.ProjectId == p.Id &&
                                   t.UserId == userId &&
                                   t.EndTime == null)
                })
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<ProjectDto> CreateAsync(string name, int userId)
        {
            var currentUserId = RequireUser();
            if (!_current.IsAdmin && currentUserId != userId)
                throw new UnauthorizedAccessException("Cannot create entry for another user.");

            var proj = new Project
            {
                Name = name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(proj);
            await _repo.SaveAsync();

            // Fresh project has no time entries yet → not running
            return new ProjectDto
            {
                Id = proj.Id,
                UserId = proj.UserId,
                Name = proj.Name,
                Correction = proj.Correction,
                CreatedAt = new DateTimeOffset(proj.CreatedAt, TimeSpan.Zero),
                IsRunning = false
            };
        }

        public async Task<ProjectDto> UpdateAsync(int projectId, string name)
        {
            var project = await Scoped()
                .FirstOrDefaultAsync(p => p.Id == projectId)
                ?? throw new NotFoundException("Project not found");

            project.Name = name;
            await _repo.SaveAsync();

            // Compute IsRunning with a cheap EXISTS that uses the (UserId, ProjectId, EndTime) index
            var isRunning = await _repo.Query()
                .Where(p => p.Id == project.Id)
                .Select(p => _repoTimes.Query().Any(t =>
                    t.UserId == p.UserId &&
                    t.ProjectId == p.Id &&
                    t.EndTime == null))
                .FirstAsync();

            return new ProjectDto
            {
                Id = project.Id,
                UserId = project.UserId,
                Name = project.Name,
                Correction = project.Correction,
                CreatedAt = new DateTimeOffset(project.CreatedAt, TimeSpan.Zero),
                IsRunning = isRunning
            };
        }
        public async Task DeleteAsync(int id)
        {
            var project = await Scoped().FirstOrDefaultAsync(p => p.Id == id) ??
                throw new NotFoundException("Project not found");
            _repo.Remove(project);
            await _repo.SaveAsync();
        }
    }
}
