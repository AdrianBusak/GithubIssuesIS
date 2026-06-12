namespace GithubIssuesIS.API.Dtos.Auth;

public sealed record AuthResponse(
    string Token,
    string Username,
    string Role,
    DateTime ExpiresAt);
