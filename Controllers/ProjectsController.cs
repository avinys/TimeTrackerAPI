using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/projects")]
    public class ProjectsController : BaseController
    {
        private readonly IProjectService _service;

        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _service.GetProjectsAsync();
            return Ok(projects);
        }

        // GET: api/projects/user?userId=123
        [HttpGet("user")]
        public async Task<IActionResult> GetProjectsByUserId([FromQuery] int? userId)
        {
            var effectiveUserId = userId ?? GetUserIdFromClaims();
            var projects = await _service.GetByUserIdAsync(effectiveUserId);
            return Ok(projects);
        }

        // GET: api/projects/{projectId}
        [HttpGet("{projectId:int}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            var project = await _service.GetByIdAsync(projectId);
            return project is null ? NotFound("Project not found") : Ok(project);
        }

        // POST: api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            var userId = dto.UserId ?? GetUserIdFromClaims();
            var project = await _service.CreateAsync(dto.Name, userId);
            return CreatedAtAction(nameof(GetProjectById), new { projectId = project.Id }, project);
        }

        // PUT: api/projects/{projectId}
        [HttpPut("{projectId:int}")]
        public async Task<IActionResult> UpdateProject(int projectId, UpdateProjectDto dto)
        {
            var name = dto.Name;
            var project = await _service.UpdateAsync(projectId, name);
            return Ok(project);
        }

        // DELETE: api/projects/{projectId}
        [HttpDelete("{projectId:int}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            await _service.DeleteAsync(projectId);
            return NoContent();
        }
    }
  
}
