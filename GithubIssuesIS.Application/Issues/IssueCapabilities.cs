namespace GithubIssuesIS.Application.Issues;

public sealed record IssueCapabilities(
    string Source,
    bool SupportsDelete,
    bool RequiresNumberOnCreate);
