namespace GithubIssuesIS.Application.Auth;

public sealed record AccessTokenResult(
    string Token,
    DateTime ExpiresAt);
