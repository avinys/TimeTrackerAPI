using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Models;
using TimeTrackerAPI.Services;
using TimeTrackerAPI.Services.Interfaces;
using static TimeTrackerAPI.Controllers.AuthController;

namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IOneTimeTokenService _tokens;
        private readonly IEmailService _emailService;
        private readonly IPasswordResetTokenService _passwordResetTokenService;
        private readonly IConfiguration _configuration;
        public record GoogleLoginDto(string IdToken);
        public record ResendDto(string Email);

        public AuthController(IUserService userService, 
            IJwtService jwtService, 
            IConfiguration configuration, 
            IOneTimeTokenService tokens, 
            IEmailService emailService, 
            IPasswordResetTokenService passwordResetTokenService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _configuration = configuration;
            _tokens = tokens;
            _emailService = emailService;
            _passwordResetTokenService = passwordResetTokenService;
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(id)) return Unauthorized();

            return Ok(new
            {
                id,
                username = User.FindFirstValue(ClaimTypes.Name),
                email = User.FindFirstValue(ClaimTypes.Email),
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserDto dto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(dto.Username, dto.Email, dto.Password);

                var (rawToken, hashHex, expiresAtUtc) = _tokens.Generate(TimeSpan.FromHours(24));
                user.EmailConfirmed = false;
                user.EmailConfirmationTokenHash = hashHex;
                user.EmailConfirmationExpiresAt = expiresAtUtc;

                // init resend window
                user.EmailConfirmationResendCountWindowStart = DateTime.UtcNow;
                user.EmailConfirmationResendCount = 1;
                user.EmailConfirmationLastSentAt = DateTime.UtcNow;

                await _userService.SaveAsync(user);

                var link = BuildConfirmLink(user.Id, rawToken);
                await _emailService.SendAsync(
                    user.Email,
                    "Confirm your email",
                    $@"<p>Hi {user.Username},</p>
                       <p>Click the button below to confirm your email address.</p>
                       <p><a href=""{link}"" style=""background:#2563eb;color:#fff;padding:10px 16px;border-radius:6px;text-decoration:none"">Confirm Email</a></p>
                       <p>This link will expire in 24 hours.</p>"
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
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.ValidateLoginAsync(dto.Identifier, dto.Password);
            if (user == null) return Unauthorized();

            if (!user.EmailConfirmed)
                return StatusCode(403, new { code = "email_not_confirmed", message = "Please confirm your email." });

            var token = _jwtService.GenerateToken(user);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                Path = "/"
            });

            return Ok(new { user.Id, user.Username, user.Email, user.Role });
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> Google([FromBody] GoogleLoginDto googleDto)
        {
            if (string.IsNullOrWhiteSpace(googleDto.IdToken))
                return BadRequest(new { message = "Missing idToken" });

            var clientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
                return StatusCode(500, new { message = "Google ClientId not configured" });

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    googleDto.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { clientId } });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid Google token" });
            }

            if (payload.EmailVerified != true)
                return Unauthorized(new { message = "Unverified google email" });

            var user = await _userService.CreateOrLinkExternalUserAsync(
                provider: "google",
                providerUserId: payload.Subject,
                email: payload.Email,
                fullName: payload.Name
            );

            // auto-confirm Google users
            user.EmailConfirmed = true;
            user.EmailConfirmationTokenHash = null;
            user.EmailConfirmationExpiresAt = null;
            await _userService.SaveAsync(user);

            var token = _jwtService.GenerateToken(user);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                Path = "/"
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

        // GET: /api/auth/confirm?userId=...&token=...
        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> Confirm([FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user is null) return NotFound();

            if (user.EmailConfirmed)
                return Ok(new { message = "Already confirmed" });

            if (user.EmailConfirmationExpiresAt is null || user.EmailConfirmationExpiresAt < DateTime.UtcNow)
                return BadRequest(new { message = "Token expired" });

            if (string.IsNullOrWhiteSpace(user.EmailConfirmationTokenHash) ||
                !_tokens.Verify(token, user.EmailConfirmationTokenHash))
                return BadRequest(new { message = "Invalid token" });

            user.EmailConfirmed = true;
            user.EmailConfirmationTokenHash = null;
            user.EmailConfirmationExpiresAt = null;
            await _userService.SaveAsync(user);

            return Ok();
        }

        // POST: /api/auth/resend-confirmation
        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> Resend([FromBody] ResendDto dto)
        {
            var email = dto.Email?.Trim().ToLowerInvariant();
            var user = await _userService.GetByEmailAsync(email);
            if (user is null || user.EmailConfirmed) return Ok();

            const int MAX = 3;
            var window = TimeSpan.FromMinutes(15);
            var now = DateTime.UtcNow;

            if (user.EmailConfirmationResendCountWindowStart is null ||
                now - user.EmailConfirmationResendCountWindowStart > window)
            {
                user.EmailConfirmationResendCountWindowStart = now;
                user.EmailConfirmationResendCount = 0;
            }

            if (user.EmailConfirmationResendCount >= MAX)
            {
                await _userService.SaveAsync(user);
                return Ok(); // silently rate-limited
            }

            var (rawToken, hashHex, expiresAtUtc) = _tokens.Generate(TimeSpan.FromHours(24));
            user.EmailConfirmationTokenHash = hashHex;
            user.EmailConfirmationExpiresAt = expiresAtUtc;
            user.EmailConfirmationResendCount += 1;
            user.EmailConfirmationLastSentAt = now;

            await _userService.SaveAsync(user);

            var link = BuildConfirmLink(user.Id, rawToken);
            await _emailService.SendAsync(
                user.Email,
                "Your confirmation link",
                $@"<p>Hi {user.Username},</p>
                   <p>Here's your new confirmation link:</p>
                   <p><a href=""{link}"">Confirm Email</a></p>
                   <p>Expires in 24 hours.</p>"
            );

            return Ok();
        }

        private string BuildConfirmLink(int userId, string rawToken)
        {
            var baseUrl = _configuration["App:PublicUrl"] ?? "https://localhost:5173";
            var enc = Uri.EscapeDataString(rawToken);
            return $"{baseUrl}/confirm?userId={userId}&token={enc}";
        }

        [HttpPost("password/forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userService.GetByEmailAsync(email);

            if (user is not null)
            {
                // generate token
                var (rawToken, tokenHash, expiresAtUtc) = _tokens.Generate(TimeSpan.FromMinutes(30));

                // persist token
                await _passwordResetTokenService.CreateAsync(
                    user.Id,
                    tokenHash,
                    expiresAtUtc,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers.UserAgent.ToString(),
                    "password-reset"
                );

                // compose link and send
                var baseUrl = _configuration["App:PublicUrl"] ?? "https://localhost:5173";
                var link = $"{baseUrl}/reset-password?uid={user.Id}&token={rawToken}";

                await _emailService.SendAsync(
                    user.Email,
                    "Reset your password",
                    $"Click this link to reset your password: {link}\n\n" +
                    "If you didn't request this, you can ignore the email."
                );
            }

            return Ok(new { message = "If the email exists, you will receive a reset link shortly." });
        }

        [HttpPost("password/reset")]
        [AllowAnonymous]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordDto dto)
        {
            var user = await _userService.GetByIdAsync(dto.UserId);
            if (user is null)
                return BadRequest(new { message = "Invalid token or user." });

            // hash the incoming raw token to lookup
            var tokenHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(dto.Token ?? string.Empty))
            );

            // get a valid (unconsumed, unexpired) token via service
            var prt = await _passwordResetTokenService.GetValidAsync(dto.UserId, tokenHash, "password-reset");
            if (prt is null)
                return BadRequest(new { message = "Invalid or expired token." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userService.SaveAsync(user);

            // consume this token and invalidate any others
            await _passwordResetTokenService.ConsumeAsync(prt);
            await _passwordResetTokenService.InvalidateOthersAsync(dto.UserId, prt.Id);

            return Ok(new { message = "Password updated successfully." });
        }

    }
}
