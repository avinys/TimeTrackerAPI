using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TimeTrackerAPI.Data;
using TimeTrackerAPI.Middlewares;
using TimeTrackerAPI.Repositories;
using TimeTrackerAPI.Repositories.Interfaces;
using TimeTrackerAPI.Services;
using TimeTrackerAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    //var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
    options.UseMySql(
        cs,
        ServerVersion.AutoDetect(cs),
        //serverVersion,
        b => b.EnableRetryOnFailure()
    );
});

// DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectTimeService, ProjectTimeService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectTimeRepository, ProjectTimeRepository>();
builder.Services.AddTransient<ApiExceptionMiddleware>();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/var/aspnet-dp-keys"))
    .SetApplicationName("TimeTrackerAPI"); // same name across instances


// Auth
builder.Services.AddJwtCookieAuth(builder.Configuration);
builder.Services.AddAuthorization();

// CORS (credentials allowed for cookie auth)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Apply EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed");
    }
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiExceptionMiddleware>();

// CORS must run before auth/authorization when using credentials
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
