using GithubIssuesIS.Application.Services;
using GithubIssuesIS.Repository;

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
        services.AddScoped<IssueService>();
        services.AddCors(options =>
        {
            options.AddPolicy(ClientCorsPolicy, policy =>
            {
                policy
                    .WithOrigins(
                        "https://localhost:7200",
                        "http://localhost:5100")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
