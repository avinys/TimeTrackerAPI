using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TimeTrackerAPI.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID claim is missing.");

            return int.Parse(userIdClaim.Value);
        }
    }
}
