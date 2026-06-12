using Microsoft.EntityFrameworkCore;

using GitHubIssuesIS.Domain.Entities;

namespace GithubIssuesIS.Repository;

public class GithubIssuesIsDbContext : DbContext
{
    public GithubIssuesIsDbContext(DbContextOptions<GithubIssuesIsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Issue> Issues => Set<Issue>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GithubIssuesIsDbContext).Assembly);
    }
}
