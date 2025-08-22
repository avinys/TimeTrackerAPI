using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services.Interfaces;
using TimeTrackerAPI.Exceptions;

namespace TimeTrackerAPI.Services
{
    public class ProjectTimeService : IProjectTimeService
    {
        private readonly IProjectTimeRepository _repo;
        private readonly IProjectService _projectService;

        public ProjectTimeService(IProjectTimeRepository repository, IProjectService projectService)
        {
            _repo = repository;
            _projectService = projectService;
        }

        public IEnumerable<ProjectTime> GetProjectTimes()
        {
            return _repo.GetProjectTimes();
        }

        public ProjectTime? GetById(int id)
        {
            return _repo.GetById(id);
        }

        public IEnumerable<ProjectTime> GetByUserAndProjectId(int userId, int projectId)
        {
            return _repo.GetByUserAndProjectId(userId, projectId);
        }

        public ProjectTime Create(int userId, int projectId)
        {
            var project = _projectService.GetById(projectId);
            if (project == null)
                throw new NotFoundException("Project not found");

            if (project.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this project");

            var projectTime = new ProjectTime
            {
                UserId = userId,
                ProjectId = projectId,
                StartTime = DateTime.UtcNow,
                EndTime = null
            };
            _repo.Add(projectTime);
            _repo.Save();
            return projectTime;
        }

        public ProjectTime Update(int projectTimeId, int userId, DateTime startTime, DateTime endTime)
        {
            var project = _repo.GetById(projectTimeId);
            if (project == null)
                throw new NotFoundException("Project not found");

            if (project.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to this project");

            project.StartTime = startTime;
            project.EndTime = endTime;

            Console.WriteLine("Updated projectTime (in service): ", project.StartTime, project.EndTime);
            _repo.Update(project);
            _repo.Save();
            return project;
        }

        public void Delete(int id, int userId)
        {
            var projectTime = _repo.GetById(id);
            Console.WriteLine("ProjectTimeService: requesting getById of prpject time for Delete method, projectTimeId: " + id + ", userId: " +  userId);
            if (projectTime == null)
                throw new NotFoundException("Project time not found.");
            if (projectTime.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete this entry");

            _repo.Delete(id);
            _repo.Save();
        }
    }
}
