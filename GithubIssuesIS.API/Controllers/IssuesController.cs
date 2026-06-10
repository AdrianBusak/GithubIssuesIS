using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.API.Dtos.Issues;
using GithubIssuesIS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GithubIssuesIS.API.Controllers;

[ApiController]
[Route("api/issues")]
public class IssuesController(IIssueService issueService) : ControllerBase
{
    private readonly IIssueService _issueService = issueService;

    [HttpGet]
    public async Task<ActionResult<List<IssueResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var issues = await _issueService.GetAllAsync(cancellationToken);

        return Ok(issues
            .OrderBy(issue => issue.Number)
            .Select(ToResponse)
            .ToList());
    }

    [HttpGet("{number:int}")]
    public async Task<ActionResult<IssueResponse>> GetByNumber(
        int number,
        CancellationToken cancellationToken)
    {
        var issue = await _issueService.GetByNumberAsync(number, cancellationToken);

        if (issue is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(issue));
    }

    [HttpPost]
    public async Task<ActionResult<IssueResponse>> Create(
        CreateIssueRequest request,
        CancellationToken cancellationToken)
    {
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
    }

    [HttpPut("{number:int}")]
    public async Task<ActionResult<IssueResponse>> Update(
        int number,
        UpdateIssueRequest request,
        CancellationToken cancellationToken)
    {
        var issue = await _issueService.UpdateAsync(number, ToIssue(request), cancellationToken);

        if (issue is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(issue));
    }

    [HttpDelete("{number:int}")]
    public async Task<IActionResult> Delete(
        int number,
        CancellationToken cancellationToken)
    {
        var deleted = await _issueService.DeleteAsync(number, cancellationToken);

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
            Number = request.Number,
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
}
