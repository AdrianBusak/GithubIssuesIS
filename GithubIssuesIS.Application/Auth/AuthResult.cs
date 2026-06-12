namespace GithubIssuesIS.Application.Auth;

public sealed record AuthResult(
    bool Succeeded,
    string? Error,
    string? AccessToken,
    string? RefreshToken,
    string? Username,
    string? Role,
    DateTime? AccessTokenExpiresAt,
    DateTime? RefreshTokenExpiresAt)
{
    public static AuthResult Success(
        string accessToken,
        string refreshToken,
        string username,
        string role,
        DateTime accessTokenExpiresAt,
        DateTime refreshTokenExpiresAt)
    {
        return new AuthResult(
            true,
            null,
            accessToken,
            refreshToken,
            username,
            role,
            accessTokenExpiresAt,
            refreshTokenExpiresAt);
    }

    public static AuthResult Failed(string error)
    {
        return new AuthResult(
            false,
            error,
            null,
            null,
            null,
            null,
            null,
            null);
    }
}
