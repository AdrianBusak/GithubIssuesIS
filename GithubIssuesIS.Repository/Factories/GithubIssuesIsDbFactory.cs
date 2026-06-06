using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GithubIssuesIS.Repository.Factories;

public class GithubIssuesIsDbFactory : IDesignTimeDbContextFactory<GithubIssuesIsDbContext>
{
    public GithubIssuesIsDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString(args)
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        var dbOptions = GenerateDbOptions(connectionString);

        return new GithubIssuesIsDbContext(dbOptions);
    }

    private static DbContextOptions<GithubIssuesIsDbContext> GenerateDbOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GithubIssuesIsDbContext>();

        GithubIssuesIsDbContextOptions.ConfigureSqlServer(optionsBuilder, connectionString);

        return optionsBuilder.Options;
    }

    private static string? ResolveConnectionString(string[] args)
    {
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            return args[0];
        }

        var environmentConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (!string.IsNullOrWhiteSpace(environmentConnectionString))
        {
            return environmentConnectionString;
        }

        return TryReadConnectionStringFromAppSettings();
    }

    private static string? TryReadConnectionStringFromAppSettings()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            var candidates = new[]
            {
                Path.Combine(directory.FullName, "appsettings.json"),
                Path.Combine(directory.FullName, "GithubIssuesIS.API", "appsettings.json")
            };

            foreach (var candidate in candidates)
            {
                var connectionString = TryReadConnectionString(candidate);

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    return connectionString;
                }
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string? TryReadConnectionString(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(path));

        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings))
        {
            return null;
        }

        if (!connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection))
        {
            return null;
        }

        return defaultConnection.GetString();
    }
}
