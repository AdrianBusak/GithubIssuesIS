using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Interfaces;
using GithubIssuesIS.Application.Issues;

namespace DMS.Infrastrucure.GitHub.Services;

public class GitHubIssueService(
    HttpClient httpClient,
    GitHubIssueSettings gitHubSettings) : IIssueService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient = httpClient;
    private readonly GitHubIssueSettings _settings = gitHubSettings;

    public IssueCapabilities Capabilities { get; } = new(
        IssueSources.GitHub,
        SupportsDelete: false,
        RequiresNumberOnCreate: false);

    private string IssuesPath =>
        $"repos/{Uri.EscapeDataString(_settings.Owner)}/{Uri.EscapeDataString(_settings.Repository)}/issues";

    public async Task<List<Issue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var issues = new List<Issue>();
        var page = 1;

        while (true)
        {
            var response = await _httpClient.GetAsync(
                $"{IssuesPath}?state=all&per_page=100&page={page}",
                cancellationToken);

            await EnsureGitHubSuccessAsync(response, cancellationToken);

            var gitHubIssues = await response.Content.ReadFromJsonAsync<List<GitHubIssueResponse>>(
                JsonOptions,
                cancellationToken) ?? [];

            issues.AddRange(gitHubIssues
                .Where(issue => issue.PullRequest is null)
                .Select(ToIssue));

            if (gitHubIssues.Count < 100)
            {
                break;
            }

            page++;
        }

        return issues;
    }

    public async Task<Issue?> GetByNumberAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{IssuesPath}/{number}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureGitHubSuccessAsync(response, cancellationToken);

        var gitHubIssue = await response.Content.ReadFromJsonAsync<GitHubIssueResponse>(
            JsonOptions,
            cancellationToken);

        if (gitHubIssue?.PullRequest is not null)
        {
            return null;
        }

        return gitHubIssue is null
            ? null
            : ToIssue(gitHubIssue);
    }

    public async Task<Issue> CreateAsync(
        Issue issue,
        CancellationToken cancellationToken = default)
    {
        var request = new GitHubIssueMutationRequest(
            issue.Title,
            issue.Body,
            State: null);

        using var response = await _httpClient.PostAsJsonAsync(
            IssuesPath,
            request,
            JsonOptions,
            cancellationToken);

        await EnsureGitHubSuccessAsync(response, cancellationToken);

        var gitHubIssue = await response.Content.ReadFromJsonAsync<GitHubIssueResponse>(
            JsonOptions,
            cancellationToken);

        return gitHubIssue is null
            ? throw new InvalidOperationException("GitHub did not return the created issue.")
            : ToIssue(gitHubIssue);
    }

    public async Task<Issue?> UpdateAsync(
        int number,
        Issue updatedIssue,
        CancellationToken cancellationToken = default)
    {
        var request = new GitHubIssueMutationRequest(
            updatedIssue.Title,
            updatedIssue.Body,
            updatedIssue.State);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Patch, $"{IssuesPath}/{number}")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureGitHubSuccessAsync(response, cancellationToken);

        var gitHubIssue = await response.Content.ReadFromJsonAsync<GitHubIssueResponse>(
            JsonOptions,
            cancellationToken);

        return gitHubIssue is null
            ? null
            : ToIssue(gitHubIssue);
    }

    public Task<bool> DeleteAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        return CloseAsync(number, cancellationToken);
    }

    private async Task<bool> CloseAsync(
        int number,
        CancellationToken cancellationToken)
    {
        var issue = await GetByNumberAsync(number, cancellationToken);

        if (issue is null)
        {
            return false;
        }

        issue.State = "closed";
        issue.ClosedAt = DateTime.UtcNow;

        await UpdateAsync(number, issue, cancellationToken);

        return true;
    }

    private static Issue ToIssue(GitHubIssueResponse issue)
    {
        return new Issue
        {
            Id = issue.Id > int.MaxValue ? 0 : (int)issue.Id,
            Number = issue.Number,
            Title = issue.Title,
            Body = issue.Body,
            State = issue.State,
            AuthorLogin = issue.User?.Login,
            HtmlUrl = issue.HtmlUrl,
            CreatedAt = issue.CreatedAt.UtcDateTime,
            ClosedAt = issue.ClosedAt?.UtcDateTime
        };
    }

    private static async Task EnsureGitHubSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized =>
                "GitHub token is invalid or expired.",
            HttpStatusCode.Forbidden =>
                "GitHub token does not have enough permissions for this repository, or the GitHub API rate limit was reached.",
            HttpStatusCode.NotFound =>
                "GitHub repository was not found, Issues are not accessible, or the token does not have access to it.",
            HttpStatusCode.Gone =>
                "GitHub Issues are disabled for this repository.",
            _ =>
                "GitHub API request failed."
        };

        throw new IssueProviderException(message, response.StatusCode);
    }

    private sealed record GitHubIssueMutationRequest(
        string Title,
        string? Body,
        string? State);

    private sealed class GitHubIssueResponse
    {
        public long Id { get; init; }

        public int Number { get; init; }

        public string Title { get; init; } = string.Empty;

        public string? Body { get; init; }

        public string State { get; init; } = "open";

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; init; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }

        [JsonPropertyName("closed_at")]
        public DateTimeOffset? ClosedAt { get; init; }

        public GitHubUserResponse? User { get; init; }

        [JsonPropertyName("pull_request")]
        public object? PullRequest { get; init; }
    }

    private sealed class GitHubUserResponse
    {
        public string? Login { get; init; }
    }
}
