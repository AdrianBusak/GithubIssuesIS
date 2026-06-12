using System.Security.Cryptography;

namespace GithubIssuesIS.Application.Helpers;

public static class PasswordHelper
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static PasswordHashResult HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return new PasswordHashResult(
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt));
    }

    public static bool VerifyPassword(
        string password,
        string passwordHash,
        string passwordSalt)
    {
        var salt = Convert.FromBase64String(passwordSalt);
        var expectedHash = Convert.FromBase64String(passwordHash);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
