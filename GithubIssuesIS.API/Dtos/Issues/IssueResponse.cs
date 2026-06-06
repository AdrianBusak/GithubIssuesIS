namespace GithubIssuesIS.API.Dtos.Issues;

public sealed record IssueResponse(
    int Id,
    long GithubId,
    int Number,
    string Title,
    string? Body,
    string State,
    string? AuthorLogin,
    string? HtmlUrl,
    DateTime CreatedAt,
    DateTime? ClosedAt);
