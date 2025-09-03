using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeTrackerAPI.Exceptions;

namespace TimeTrackerAPI.Middlewares
{
    public class ApiExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ApiExceptionMiddleware> _logger;
        public ApiExceptionMiddleware(ILogger<ApiExceptionMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
        {
            try
            {
                await next(ctx);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteProblem(ctx, StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await WriteProblem(ctx, StatusCodes.Status403Forbidden, "Forbidden", ex.Message);
            }
            catch (NotFoundException ex)
            {
                await WriteProblem(ctx, StatusCodes.Status404NotFound, "Not Found", ex.Message);
            }
            catch (ValidationException ex)
            {
                await WriteProblem(ctx, StatusCodes.Status400BadRequest, "Validation error", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                await WriteProblem(ctx, StatusCodes.Status500InternalServerError, "Server error",
                    "An unexpected error occurred.");
            }
        }

        private static Task WriteProblem(HttpContext ctx, int status, string title, string detail)
        {
            if (ctx.Response.HasStarted)
            {
                // If headers/body already sent, we can’t write a problem payload safely.
                return Task.CompletedTask;
            }
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/problem+json";
            return ctx.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = title, Detail = detail });
        }
    }
}
