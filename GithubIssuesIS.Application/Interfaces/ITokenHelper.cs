using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Auth;

namespace GithubIssuesIS.Application.Interfaces;

public interface ITokenHelper
{
    AccessTokenResult GenerateAccessToken(User user);

    RefreshTokenResult GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}
