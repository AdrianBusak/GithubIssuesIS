using DMS.Infrastrucure.JwtAuthorization.Extensions;
using GithubIssuesIS.Application.Interfaces;
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
        services.AddScoped<IIssueService, IssueService>();
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
