using TimeTrackerAPI.Models;
using TimeTrackerAPI.Services.Interfaces;
using TimeTrackerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;




namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTimeController : BaseController
    {
        private readonly IProjectTimeService _service;
        private static ProjectTimeDto ToDto(ProjectTime e) => new()
        {
            Id = e.Id,
            UserId = e.UserId,
            ProjectId = e.ProjectId,
            // mark as UTC so JSON gets a trailing Z
            StartTime = new DateTimeOffset(
        DateTime.SpecifyKind(e.StartTime, DateTimeKind.Utc)),
            EndTime = e.EndTime.HasValue
        ? new DateTimeOffset(DateTime.SpecifyKind(e.EndTime.Value, DateTimeKind.Utc))
        : null
        };

        public ProjectTimeController(IProjectTimeService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetProjectTimes()
        {
            return Ok(_service.GetProjectTimes().Select(ToDto));
        }

        [HttpGet("user-project/{projectId}")]
        public IActionResult GetProjectsByUserAndProjectId(int projectId)
        {
            var userId = GetUserIdFromClaims();
            return Ok(_service.GetByUserAndProjectId(userId, projectId).Select(ToDto));
        }

        [HttpGet("{projectTimeId}")]
        public IActionResult GetProjectTimeById(int projectTimeId)
        {
            var projectTime = _service.GetById(projectTimeId);
            if (projectTime == null)
            {
                return NotFound();
            }
            return Ok(ToDto(projectTime));
        }

        [HttpPost]
        public IActionResult CreateProjectTime(CreateProjectTimeDto dto)
        {
            var userId = GetUserIdFromClaims();
            var projectTime = _service.Create(userId, dto.ProjectId);
            return CreatedAtAction(nameof(GetProjectTimeById), new { projectTimeId = projectTime.Id }, ToDto(projectTime));
        }

        [HttpPut("{projectTimeId}")]
        public IActionResult UpdateProjectTime(int projectTimeId, UpdateProjectTimeDto dto)
        {
            var userId = GetUserIdFromClaims();
            var startTimeUtc = dto.StartTime.UtcDateTime;
            var endTime = DateTime.UtcNow;
            var projectTime = _service.Update(projectTimeId, userId, startTimeUtc, endTime);
            return Ok(ToDto(projectTime));
        }

        [HttpDelete("{projectTimeId}")]
        public IActionResult DeleteProjectTimeById(int projectTimeId)
        {
            var userId = GetUserIdFromClaims();
            _service.Delete(projectTimeId, userId);
            return NoContent();
        }
    }
}
