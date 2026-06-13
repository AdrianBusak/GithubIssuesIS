namespace GitHubIssuesIS.Client.Models;

public sealed class SoapIssueDto
{
    public int Number { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Body { get; set; }

    public string State { get; set; } = string.Empty;

    public string? AuthorLogin { get; set; }

    public string? HtmlUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}
