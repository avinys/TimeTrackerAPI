using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.Data;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_service.GetUsers());
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpPost]
        public IActionResult CreateUser(CreateUserDto dto)
        {
            var user = _service.CreateUser(dto.Username, dto.Email, dto.Password);
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
    }
}
