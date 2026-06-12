namespace GithubIssuesIS.Application.Interfaces;

public interface IAuthSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
