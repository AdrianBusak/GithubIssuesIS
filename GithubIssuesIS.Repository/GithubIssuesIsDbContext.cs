using Microsoft.EntityFrameworkCore;

namespace GithubIssuesIS.Repository;

public class GithubIssuesIsDbContext : DbContext
{
    public GithubIssuesIsDbContext(DbContextOptions<GithubIssuesIsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GithubIssuesIsDbContext).Assembly);
    }
}