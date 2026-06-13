using GitHubIssuesIS.Domain.Entities;

namespace GithubIssuesIS.API.GraphQL;

public sealed record IssueGraphQlDto(
    int Id,
    int Number,
    string Title,
    string? Body,
    string State,
    string? AuthorLogin,
    string? HtmlUrl,
    DateTime CreatedAt,
    DateTime? ClosedAt);

public sealed class CreateIssueInput
{
    public int? Number { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Body { get; init; }

    public string State { get; init; } = "open";

    public string? AuthorLogin { get; init; }

    public string? HtmlUrl { get; init; }
}

public sealed class UpdateIssueInput
{
    public string Title { get; init; } = string.Empty;

    public string? Body { get; init; }

    public string State { get; init; } = "open";

    public string? AuthorLogin { get; init; }

    public string? HtmlUrl { get; init; }

    public DateTime? ClosedAt { get; init; }
}

public static class IssueGraphQlMapper
{
    public static IssueGraphQlDto ToGraphQlDto(this Issue issue)
    {
        return new IssueGraphQlDto(
            issue.Id,
            issue.Number,
            issue.Title,
            issue.Body,
            issue.State,
            issue.AuthorLogin,
            issue.HtmlUrl,
            issue.CreatedAt,
            issue.ClosedAt);
    }

    public static Issue ToIssue(this CreateIssueInput input)
    {
        return new Issue
        {
            Number = input.Number.GetValueOrDefault(),
            Title = input.Title,
            Body = input.Body,
            State = input.State,
            AuthorLogin = input.AuthorLogin,
            HtmlUrl = input.HtmlUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Issue ToIssue(this UpdateIssueInput input)
    {
        return new Issue
        {
            Title = input.Title,
            Body = input.Body,
            State = input.State,
            AuthorLogin = input.AuthorLogin,
            HtmlUrl = input.HtmlUrl,
            ClosedAt = input.ClosedAt
        };
    }
}
