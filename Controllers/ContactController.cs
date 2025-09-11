using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.DTOs;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/contact")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _email;
        private readonly IConfiguration _config;
        public ContactController(IEmailService email, IConfiguration configuration)
        {
            _email = email;
            _config = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromBody] ContactUsDto dto)
        {
            // bot guard
            if (!string.IsNullOrWhiteSpace(dto.Website))
                return Ok(new { message = "Thanks! We'll be in touch." });

            var to = _config["App:ContactEmail"] ?? "boss@arvydasvingis.com";
            var subject = $"[TimeTracker Contact] {(!string.IsNullOrWhiteSpace(dto.Subject) ? dto.Subject : "New message")} — {dto.Name}";

            string E(string s) => WebUtility.HtmlEncode(s);

            var bodyHtml = $@"
                <h3>New contact message</h3>
                <p><b>Name:</b> {E(dto.Name)}<br>
                <b>Email:</b> {E(dto.Email)}<br>
                <b>Phone:</b> {E(dto.Phone ?? "-")}<br>
                <b>IP:</b> {HttpContext.Connection.RemoteIpAddress}<br>
                <b>UA:</b> {E(Request.Headers.UserAgent)}</p>
                <p><b>Message:</b></p>
                <pre style='white-space:pre-wrap'>{E(dto.Message)}</pre>";

            await _email.SendAsync(to, subject, bodyHtml);
            return Ok(new { message = "Thanks! We'll be in touch." });
        }
    }
}
