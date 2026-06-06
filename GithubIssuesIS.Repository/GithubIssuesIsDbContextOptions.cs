using Microsoft.EntityFrameworkCore;

namespace GithubIssuesIS.Repository;

public static class GithubIssuesIsDbContextOptions
{
    public static void ConfigureSqlServer(
        DbContextOptionsBuilder optionsBuilder,
        string? connectionString = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            optionsBuilder.UseSqlServer(sqlOptions =>
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

            return;
        }

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlOptions => sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
    }
}
