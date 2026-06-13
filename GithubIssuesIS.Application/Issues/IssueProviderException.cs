using System.Net;

namespace GithubIssuesIS.Application.Issues;

public class IssueProviderException(
    string message,
    HttpStatusCode? statusCode = null) : Exception(message)
{
    public HttpStatusCode? StatusCode { get; } = statusCode;
}
