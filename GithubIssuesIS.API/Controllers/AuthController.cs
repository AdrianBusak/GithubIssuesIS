using GithubIssuesIS.API.Dtos.Auth;
using GithubIssuesIS.Application.Auth;
using GithubIssuesIS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GithubIssuesIS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    JwtSettings jwtSettings) : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthService _authService = authService;
    private readonly JwtSettings _jwtSettings = jwtSettings;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        AuthRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(
            request.Username,
            request.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Conflict(new { message = result.Error });
        }

        SetRefreshTokenCookie(result);

        return Ok(ToResponse(result));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        AuthRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            request.Username,
            request.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Unauthorized(new { message = result.Error });
        }

        SetRefreshTokenCookie(result);

        return Ok(ToResponse(result));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
        {
            return Unauthorized();
        }

        var result = await _authService.RefreshAsync(refreshToken, cancellationToken);

        if (!result.Succeeded)
        {
            ClearRefreshTokenCookie();
            return Unauthorized(new { message = result.Error });
        }

        SetRefreshTokenCookie(result);

        return Ok(ToResponse(result));
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOut(CancellationToken cancellationToken)
    {
        Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken);

        await _authService.SignOutAsync(refreshToken, cancellationToken);
        ClearRefreshTokenCookie();

        return NoContent();
    }

    private void SetRefreshTokenCookie(AuthResult result)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            result.RefreshToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.RefreshTokenExpiresAt,
                MaxAge = TimeSpan.FromDays(_jwtSettings.RefreshTokenDays),
                Path = "/"
            });
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
    }

    private static AuthResponse ToResponse(AuthResult result)
    {
        return new AuthResponse(
            result.AccessToken!,
            result.Username!,
            result.Role!,
            result.AccessTokenExpiresAt!.Value);
    }
}
