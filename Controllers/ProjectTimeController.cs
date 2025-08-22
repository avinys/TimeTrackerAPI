using TimeTrackerAPI.Models;
using TimeTrackerAPI.Services.Interfaces;
using TimeTrackerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTimeController : BaseController
    {
        private readonly IProjectTimeService _service;

        public ProjectTimeController(IProjectTimeService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetProjectTimes()
        {
            return Ok(_service.GetProjectTimes());
        }

        [HttpGet("user-project/{projectId}")]
        public IActionResult GetProjectsByUserAndProjectId(int projectId)
        {
            var userId = GetUserIdFromClaims();
            Console.WriteLine("Getting projet times with params: ", projectId, userId);
            return Ok(_service.GetByUserAndProjectId(userId, projectId));
        }

        [HttpGet("{projectTimeId}")]
        public IActionResult GetProjectTimeById(int projectTimeId)
        {
            var projectTime = _service.GetById(projectTimeId);
            if (projectTime == null)
            {
                return NotFound();
            }
            return Ok(projectTime);
        }

        [HttpPost]
        public IActionResult CreateProjectTime(CreateProjectTimeDto dto)
        {
            var userId = GetUserIdFromClaims();
            var projectTime = _service.Create(userId, dto.ProjectId);
            return CreatedAtAction(nameof(GetProjectTimeById), new { projectTimeId = projectTime.Id }, projectTime);
        }

        [HttpPut("{projectTimeId}")]
        public IActionResult UpdateProjectTime(int projectTimeId, UpdateProjectTimeDto dto)
        {
            var userId = GetUserIdFromClaims();
            var endTime = DateTime.UtcNow;
            var projectTime = _service.Update(projectTimeId, userId, dto.StartTime, endTime);
            Console.WriteLine("Updated projectTime: ", projectTime.StartTime, projectTime.EndTime);
            return Ok(projectTime);
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
