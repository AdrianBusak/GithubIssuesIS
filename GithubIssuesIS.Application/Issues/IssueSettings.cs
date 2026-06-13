namespace GithubIssuesIS.Application.Issues;

public sealed class IssueSettings
{
    public const string SectionName = "Issues";

    public string Source { get; set; } = IssueSources.Local;

    public GitHubIssueSettings GitHub { get; set; } = new();
}

public sealed class GitHubIssueSettings
{
    public string Owner { get; set; } = string.Empty;

    public string Repository { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;
}

public static class IssueSources
{
    public const string Local = "Local";

    public const string GitHub = "GitHub";
}
