using TimeTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TimeTrackerAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public int? UserId => int.TryParse(_http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (int?)null;
        public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        public bool IsAdmin => _http.HttpContext?.User?.IsInRole("Admin") ?? false;
    }
}
