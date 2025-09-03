using TimeTrackerAPI.Models;
using TimeTrackerAPI.Services.Interfaces;
using TimeTrackerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/project-times")]
    public class ProjectTimeController : BaseController
    {
        private readonly IProjectTimeService _service;
        private static ProjectTimeDto ToDto(ProjectTime e) => new()
        {
            Id = e.Id,
            UserId = e.UserId,
            ProjectId = e.ProjectId,
            StartTime = new DateTimeOffset(e.StartTime, TimeSpan.Zero), // treat as UTC
            EndTime = e.EndTime.HasValue
                ? new DateTimeOffset(e.EndTime.Value, TimeSpan.Zero)
                : (DateTimeOffset?)null,
            Comment = e.Comment
        };

        public ProjectTimeController(IProjectTimeService service)
        {
            _service = service;
        }

        // GET: api/project-times
        [HttpGet]
        public async Task<IActionResult> GetProjectTimes()
        {
            var projectTimes = await _service.GetProjectTimesAsync();
            return Ok(projectTimes.Select(ToDto));
        }

        // GET: api/project-times/user-project/{projectId}?userId=123
        // - Regular users: ignores/forbids mismatched userId
        // - Admins: can pass any userId to fetch that user's entries
        [HttpGet("user-project/{projectId:int}")]
        public async Task<IActionResult> GetByUserAndProject(int projectId, [FromQuery] int? userId)
        {
            var effectiveUserId = userId ?? GetUserIdFromClaims();
            var projectTimes = await _service.GetByUserAndProjectIdAsync(effectiveUserId,  projectId);
            return Ok(projectTimes.Select(ToDto));
        }

        // GET: api/project-times/{projectTimeId}
        [HttpGet("{projectTimeId:int}")]
        public async Task<IActionResult> GetProjectTimeById(int projectTimeId)
        {
            var projectTime = await _service.GetByIdAsync(projectTimeId);
            return projectTime is null ? NotFound("Project time was not found") : Ok(ToDto(projectTime));
        }

        // POST: api/project-times
        [HttpPost]
        public async Task<IActionResult> CreateProjectTime([FromBody] CreateProjectTimeDto dto)
        {
            var userId = dto.UserId ?? GetUserIdFromClaims();
            var projectTime = await _service.CreateAsync(userId, dto.ProjectId);
            return CreatedAtAction(nameof(GetProjectTimeById), 
                new { projectTimeId = projectTime.Id }, 
                ToDto(projectTime));
        }

        // PUT: api/project-times/{projectTimeId}
        [HttpPut("{projectTimeId:int}")]
        public async Task<IActionResult> UpdateProjectTime(int projectTimeId, UpdateProjectTimeDto dto)
        {
            var startUtc = dto.StartTime?.UtcDateTime;
            var endUtc = dto.EndTime?.UtcDateTime;
            var comment = dto.Comment ?? string.Empty;
            var projectTime = await _service.UpdateAsync(projectTimeId, startUtc, endUtc, comment);
            return Ok(ToDto(projectTime));
        }

        // PUT: api/project-times/stop/{projectTimeId}
        [HttpPut("stop/{projectTimeId:int}")]
        public async Task<IActionResult> StopProjectTime(int projectTimeId)
        {
            var endUtc = DateTimeOffset.UtcNow;
            var projectTime = await _service.UpdateAsync(projectTimeId, null, endUtc, null);
            return Ok(ToDto(projectTime));
        }

        // DELETE: api/project-times/{projectTimeId}
        [HttpDelete("{projectTimeId:int}")]
        public async Task<IActionResult> DeleteProjectTimeById(int projectTimeId)
        {
            await _service.DeleteAsync(projectTimeId);
            return NoContent();
        }
    }
}
