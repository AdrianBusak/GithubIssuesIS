namespace GithubIssuesIS.Application.Helpers;

public sealed record PasswordHashResult(
    string Hash,
    string Salt);
