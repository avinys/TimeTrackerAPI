using System.Security.Cryptography;
using System.Text;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public sealed class OneTimeTokenService : IOneTimeTokenService
    {
        public (string RawToken, string HashHex, DateTime ExpiresAtUtc) Generate(TimeSpan ttl)
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            var raw = Base64UrlEncode(bytes);
            var hashHex = Sha256Hex(raw);
            return (raw, hashHex, DateTime.UtcNow.Add(ttl));
        }

        public bool Verify(string rawToken, string storedHashHex)
            => FixedTimeEqualsHex(Sha256Hex(rawToken ?? ""), storedHashHex ?? "");

        private static string Sha256Hex(string s)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
        }

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        // Avoids subtle timing attacks (paranoia-level hardening)
        private static bool FixedTimeEqualsHex(string aHex, string bHex)
        {
            if (aHex is null || bHex is null) return false;
            if (aHex.Length != bHex.Length) return false;

            byte[] a = Convert.FromHexString(aHex);
            byte[] b = Convert.FromHexString(bHex);

            return CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}
