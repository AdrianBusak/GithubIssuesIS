using System.Runtime.Serialization;

namespace GithubIssuesIS.API.Soap.Models;

[DataContract]
public sealed class SoapIssueDto
{
    [DataMember(Order = 1)]
    public int Number { get; set; }

    [DataMember(Order = 2)]
    public string Title { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string? Body { get; set; }

    [DataMember(Order = 4)]
    public string State { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public string? AuthorLogin { get; set; }

    [DataMember(Order = 6)]
    public string? HtmlUrl { get; set; }

    [DataMember(Order = 7)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Order = 8)]
    public DateTime? ClosedAt { get; set; }
}
