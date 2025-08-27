using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : BaseController
    {
        private readonly IProjectService _service;

        public ProjectsController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            return Ok(_service.GetProjects());
        }

        [HttpGet("user")]
        public IActionResult GetProjectByUserId()
        {
            var userId = GetUserIdFromClaims();
            return Ok(_service.GetByUserId(userId));
        }

        [HttpGet("{id}")]
        public IActionResult GetProjectById(int projectId)
        {
            var project = _service.GetById(projectId);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        public IActionResult CreateProject(CreateProjectDto dto)
        {
            var userId = GetUserIdFromClaims();
            var project = _service.Create(dto.Name, userId);
            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
        }
    }
  
}
