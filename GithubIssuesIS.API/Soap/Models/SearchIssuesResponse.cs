using System.Runtime.Serialization;

namespace GithubIssuesIS.API.Soap.Models;

[DataContract]
public sealed class SearchIssuesResponse
{
    [DataMember(Order = 1)]
    public int Count { get; set; }

    [DataMember(Order = 2)]
    public List<SoapIssueDto> Issues { get; set; } = [];
}
