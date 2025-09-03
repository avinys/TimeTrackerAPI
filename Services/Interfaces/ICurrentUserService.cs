namespace TimeTrackerAPI.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}
