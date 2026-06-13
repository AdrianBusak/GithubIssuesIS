namespace GithubIssuesIS.API.Dtos.Import;

public sealed record ImportResponse(
    bool Succeeded,
    string Message,
    IReadOnlyList<string> Errors);
