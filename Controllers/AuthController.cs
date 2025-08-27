using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Services;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized();

            var id = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;


            return Ok(new { id, username, email, role });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(CreateUserDto createUserDto)
        { 
            try
            {
                var user = _userService.CreateUser(
                    createUserDto.Username, 
                    createUserDto.Email, 
                    createUserDto.Password
                );
                return Ok(new { user.Id, user.Username, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDto loginDto)
        {
            var user = _userService.ValidateLogin(loginDto.Identifier, loginDto.Password);
        
            if(user == null)
            {
                return Unauthorized();
            }

            var token = _jwtService.GenerateToken(user);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(new { user.Id, user.Username, user.Email, user.Role });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/", 
            });

            return Ok(new { message = "Logged out successfully" });
        }

    }
}
