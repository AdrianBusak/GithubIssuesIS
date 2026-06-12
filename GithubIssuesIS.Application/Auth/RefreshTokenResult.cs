namespace GithubIssuesIS.Application.Auth;

public sealed record RefreshTokenResult(
    string Token,
    string TokenHash,
    DateTime ExpiresAt);
