using System.Net.Http.Headers;
using DMS.Infrastrucure.GitHub.Services;
using GithubIssuesIS.Application.Interfaces;
using GithubIssuesIS.Application.Issues;
using Microsoft.Extensions.DependencyInjection;

namespace DMS.Infrastrucure.GitHub.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitHubIssueServices(
        this IServiceCollection services,
        GitHubIssueSettings gitHubSettings)
    {
        ValidateGitHubSettings(gitHubSettings);

        services.AddSingleton(gitHubSettings);

        services.AddHttpClient<GitHubIssueService>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<GitHubIssueSettings>();

            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("GithubIssuesIS/1.0");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                settings.AccessToken);
        });

        services.AddScoped<IIssueService>(serviceProvider =>
            serviceProvider.GetRequiredService<GitHubIssueService>());

        return services;
    }

    private static void ValidateGitHubSettings(GitHubIssueSettings gitHubSettings)
    {
        if (string.IsNullOrWhiteSpace(gitHubSettings.Owner))
        {
            throw new InvalidOperationException("Issues:GitHub:Owner is missing.");
        }

        if (string.IsNullOrWhiteSpace(gitHubSettings.Repository))
        {
            throw new InvalidOperationException("Issues:GitHub:Repository is missing.");
        }

        if (string.IsNullOrWhiteSpace(gitHubSettings.AccessToken))
        {
            throw new InvalidOperationException("Issues:GitHub:AccessToken is missing.");
        }
    }
}
