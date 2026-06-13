using System.ServiceModel;
using GithubIssuesIS.API.Soap.Models;

namespace GithubIssuesIS.API.Soap.Interfaces;

[ServiceContract]
public interface IIssueSoapService
{
    [OperationContract(Name = "SearchIssues")]
    Task<SearchIssuesResponse> SearchIssuesAsync(string searchTerm);
}
