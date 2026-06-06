using System.ComponentModel.DataAnnotations;

namespace GitHubIssuesIS.Client.Models;

public sealed class UpdateIssueDto
{
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Body { get; set; }

    [Required]
    [MaxLength(50)]
    public string State { get; set; } = "open";

    [MaxLength(150)]
    public string? AuthorLogin { get; set; }

    [MaxLength(500)]
    public string? HtmlUrl { get; set; }

    public DateTime? ClosedAt { get; set; }
}
