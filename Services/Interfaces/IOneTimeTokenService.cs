namespace TimeTrackerAPI.Services.Interfaces
{
    public interface IOneTimeTokenService
    {
        (string RawToken, string HashHex, DateTime ExpiresAtUtc) Generate(TimeSpan ttl);
        bool Verify(string rawToken, string storedHashHex);
    }

}
