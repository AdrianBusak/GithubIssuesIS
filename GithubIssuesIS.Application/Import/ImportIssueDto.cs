using System.Text.Json.Serialization;
using System.Xml.Serialization;
using GitHubIssuesIS.Domain.Entities;

namespace GithubIssuesIS.Application.Import;

[XmlRoot("issue")]
public sealed class ImportIssueDto
{
    [JsonPropertyName("number")]
    [XmlElement("number")]
    public int Number { get; set; }

    [JsonPropertyName("title")]
    [XmlElement("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    [XmlElement("body")]
    public string? Body { get; set; }

    [JsonPropertyName("state")]
    [XmlElement("state")]
    public string State { get; set; } = "open";

    [JsonPropertyName("authorLogin")]
    [XmlElement("authorLogin")]
    public string? AuthorLogin { get; set; }

    [JsonPropertyName("htmlUrl")]
    [XmlElement("htmlUrl")]
    public string? HtmlUrl { get; set; }

    [JsonPropertyName("createdAt")]
    [XmlElement("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("closedAt")]
    [XmlElement("closedAt")]
    public DateTime? ClosedAt { get; set; }

    public Issue ToEntity()
    {
        return new Issue
        {
            Number = Number,
            Title = Title,
            Body = Body,
            State = State,
            AuthorLogin = AuthorLogin,
            HtmlUrl = HtmlUrl,
            CreatedAt = CreatedAt ?? DateTime.UtcNow,
            ClosedAt = ClosedAt
        };
    }
}
