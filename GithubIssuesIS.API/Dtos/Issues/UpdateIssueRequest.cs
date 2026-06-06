using System.ComponentModel.DataAnnotations;

namespace GithubIssuesIS.API.Dtos.Issues;

public sealed class UpdateIssueRequest
{
    [Required]
    [MaxLength(300)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(4000)]
    public string? Body { get; init; }

    [Required]
    [MaxLength(50)]
    public string State { get; init; } = "open";

    [MaxLength(150)]
    public string? AuthorLogin { get; init; }

    [MaxLength(500)]
    public string? HtmlUrl { get; init; }

    public DateTime? ClosedAt { get; init; }
}
