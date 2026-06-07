using GithubIssuesIS.Application.Interfaces;
using GithubIssuesIS.Repository.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GithubIssuesIS.Repository.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        services.AddDbContext<GithubIssuesIsDbContext>(options =>
            GithubIssuesIsDbContextOptions.ConfigureSqlServer(options, connectionString));

        services.AddScoped<IRepository, RepositoryService>();

        return services;
    }
}
