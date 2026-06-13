using DMS.Infrastrucure.GitHub.Extensions;
using DMS.Infrastrucure.Import.Extensions;
using DMS.Infrastrucure.JwtAuthorization.Extensions;
using GithubIssuesIS.Application.Interfaces;
using GithubIssuesIS.Application.Issues;
using GithubIssuesIS.Application.Services;
using GithubIssuesIS.Repository.Extensions;

namespace GithubIssuesIS.API.Exstensions;

public static class ServiceCollectionExtensions
{
    public const string ClientCorsPolicy = "ClientCorsPolicy";

    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddRepository(connectionString);
        services.AddAuthServices(configuration);
        services.AddIssueServices(configuration);
        services.AddImportServices();
        services.AddCors(options =>
        {
            options.AddPolicy(ClientCorsPolicy, policy =>
            {
                policy
                    .WithOrigins(
                        "https://localhost:7200",
                        "http://localhost:5100")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    private static IServiceCollection AddIssueServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var issueSettings = configuration
            .GetSection(IssueSettings.SectionName)
            .Get<IssueSettings>() ?? new IssueSettings();

        services.AddSingleton(issueSettings);

        if (issueSettings.Source.Equals(IssueSources.Local, StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IIssueService, LocalIssueService>();
            return services;
        }

        if (!issueSettings.Source.Equals(IssueSources.GitHub, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Issues:Source must be 'Local' or 'GitHub'.");
        }

        return services.AddGitHubIssueServices(issueSettings.GitHub);
    }

    private static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddJwtAuthorization(configuration);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthSeeder, AuthSeeder>();

        return services;
    }
}
