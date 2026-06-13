using GitHubIssuesIS.Domain;
using GithubIssuesIS.API.Dtos.Import;
using GithubIssuesIS.Application.Import;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GithubIssuesIS.API.Controllers;

[ApiController]
[Route("api/import")]
[Authorize(Roles = Roles.Admin)]
public class ImportController(IImportService importService) : ControllerBase
{
    private readonly IImportService _importService = importService;

    [HttpPost]
    [Consumes("application/json", "application/xml")]
    public async Task<IActionResult> Import(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync(cancellationToken);

        var contentType = Request.ContentType?.ToLowerInvariant();

        if (contentType?.Contains("xml") == true)
        {
            return HandleResponse(await _importService.ImportXmlAsync(body, cancellationToken));
        }

        if (contentType?.Contains("json") == true)
        {
            return HandleResponse(await _importService.ImportJsonAsync(body, cancellationToken));
        }

        return BadRequest(new ImportResponse(
            false,
            "Unsupported content type. Use application/xml or application/json.",
            []));
    }

    private IActionResult HandleResponse(ImportResult result)
    {
        var response = new ImportResponse(
            result.Succeeded,
            result.Message,
            result.Errors);

        return result.Succeeded
            ? Ok(response)
            : BadRequest(response);
    }
}
