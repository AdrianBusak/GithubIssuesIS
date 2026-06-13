using GitHubIssuesIS.Domain;
using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.API.Dtos.Issues;
using GithubIssuesIS.Application.Interfaces;
using GithubIssuesIS.Application.Issues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubIssuesIS.API.Controllers;

[ApiController]
[Route("api/issues")]
public class IssuesController(IIssueService issueService) : ControllerBase
{
    private readonly IIssueService _issueService = issueService;

    [HttpGet]
    [Authorize(Roles = Roles.UserOrAdmin)]
    public async Task<ActionResult<List<IssueResponse>>> GetAll(CancellationToken cancellationToken)
    {
        List<Issue> issues;

        try
        {
            issues = await _issueService.GetAllAsync(cancellationToken);
        }
        catch (IssueProviderException ex)
        {
            return IssueProviderError(ex);
        }

        return Ok(issues
            .OrderBy(issue => issue.Number)
            .Select(ToResponse)
            .ToList());
    }

    [HttpGet("capabilities")]
    [Authorize(Roles = Roles.UserOrAdmin)]
    public ActionResult<IssueCapabilitiesResponse> GetCapabilities()
    {
        var capabilities = _issueService.Capabilities;

        return Ok(new IssueCapabilitiesResponse(
            capabilities.Source,
            capabilities.SupportsDelete,
            capabilities.RequiresNumberOnCreate));
    }

    [HttpGet("{number:int}")]
    [Authorize(Roles = Roles.UserOrAdmin)]
    public async Task<ActionResult<IssueResponse>> GetByNumber(
        int number,
        CancellationToken cancellationToken)
    {
        Issue? issue;

        try
        {
            issue = await _issueService.GetByNumberAsync(number, cancellationToken);
        }
        catch (IssueProviderException ex)
        {
            return IssueProviderError(ex);
        }

        if (issue is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(issue));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<IssueResponse>> Create(
        CreateIssueRequest request,
        CancellationToken cancellationToken)
    {
        if (_issueService.Capabilities.RequiresNumberOnCreate && request.Number is null)
        {
            return BadRequest(new { message = "Issue number is required." });
        }

        try
        {
            var issue = await _issueService.CreateAsync(ToIssue(request), cancellationToken);
            var response = ToResponse(issue);

            return CreatedAtAction(
                nameof(GetByNumber),
                new { number = response.Number },
                response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { ex.Message });
        }
        catch (IssueProviderException ex)
        {
            return IssueProviderError(ex);
        }
    }

    [HttpPut("{number:int}")]
    [HttpPatch("{number:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<IssueResponse>> Update(
        int number,
        UpdateIssueRequest request,
        CancellationToken cancellationToken)
    {
        Issue? issue;

        try
        {
            issue = await _issueService.UpdateAsync(number, ToIssue(request), cancellationToken);
        }
        catch (IssueProviderException ex)
        {
            return IssueProviderError(ex);
        }

        if (issue is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(issue));
    }

    [HttpDelete("{number:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(
        int number,
        CancellationToken cancellationToken)
    {
        bool deleted;

        try
        {
            deleted = await _issueService.DeleteAsync(number, cancellationToken);
        }
        catch (NotSupportedException ex)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed, new { ex.Message });
        }

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static Issue ToIssue(CreateIssueRequest request)
    {
        return new Issue
        {
            Number = request.Number.GetValueOrDefault(),
            Title = request.Title,
            Body = request.Body,
            State = request.State,
            AuthorLogin = request.AuthorLogin,
            HtmlUrl = request.HtmlUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Issue ToIssue(UpdateIssueRequest request)
    {
        return new Issue
        {
            Title = request.Title,
            Body = request.Body,
            State = request.State,
            AuthorLogin = request.AuthorLogin,
            HtmlUrl = request.HtmlUrl,
            ClosedAt = request.ClosedAt
        };
    }

    private static IssueResponse ToResponse(Issue issue)
    {
        return new IssueResponse(
            issue.Id,
            issue.Number,
            issue.Title,
            issue.Body,
            issue.State,
            issue.AuthorLogin,
            issue.HtmlUrl,
            issue.CreatedAt,
            issue.ClosedAt);
    }

    private ObjectResult IssueProviderError(IssueProviderException exception)
    {
        return StatusCode(
            StatusCodes.Status502BadGateway,
            new
            {
                message = exception.Message,
                source = _issueService.Capabilities.Source
            });
    }
}
