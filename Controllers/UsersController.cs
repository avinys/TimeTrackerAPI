using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        public UsersController(IUserService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _service.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var user = await _service.CreateUserAsync(dto.Username, dto.Email, dto.Password);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
    }
}
