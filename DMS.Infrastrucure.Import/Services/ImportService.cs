using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GithubIssuesIS.Application.Import;
using GithubIssuesIS.Application.Interfaces;
using Json.Schema;

namespace DMS.Infrastrucure.Import.Services;

public class ImportService(IIssueRepository issueRepository) : IImportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IIssueRepository _issueRepository = issueRepository;
    private readonly string _schemaDir = Path.Combine(AppContext.BaseDirectory, "Schemas");

    public async Task<ImportResult> ImportJsonAsync(
        string jsonContent,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = await ValidateJsonAsync(jsonContent, cancellationToken);

        if (validationErrors.Count > 0)
        {
            return ImportResult.Failure("JSON validation failed.", validationErrors);
        }

        ImportIssueDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<ImportIssueDto>(jsonContent, JsonOptions);
        }
        catch (JsonException ex)
        {
            return ImportResult.Failure("JSON deserialization failed.", [ex.Message]);
        }

        return await SaveAsync(dto, cancellationToken);
    }

    public async Task<ImportResult> ImportXmlAsync(
        string xmlContent,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = ValidateXml(xmlContent, out var dto);

        if (validationErrors.Count > 0)
        {
            return ImportResult.Failure("XML validation failed.", validationErrors);
        }

        return await SaveAsync(dto, cancellationToken);
    }

    private async Task<List<string>> ValidateJsonAsync(
        string jsonContent,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var schemaPath = Path.Combine(_schemaDir, "issue-schema.json");
        var schemaText = await File.ReadAllTextAsync(schemaPath, cancellationToken);
        var schema = JsonSchema.FromText(schemaText);

        JsonNode? node;

        try
        {
            node = JsonNode.Parse(jsonContent);
        }
        catch (JsonException ex)
        {
            errors.Add(ex.Message);
            return errors;
        }

        var results = schema.Evaluate(
            node,
            new EvaluationOptions
            {
                OutputFormat = OutputFormat.List
            });

        if (!results.IsValid)
        {
            errors.Add("JSON content does not match issue-schema.json.");
        }

        return errors;
    }

    private List<string> ValidateXml(
        string xmlContent,
        out ImportIssueDto? dto)
    {
        dto = null;
        var errors = new List<string>();
        var schemaSet = new XmlSchemaSet();
        var schemaPath = Path.Combine(_schemaDir, "issue-schema.xsd");

        using (var schemaReader = XmlReader.Create(schemaPath))
        {
            schemaSet.Add(null, schemaReader);
        }

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemaSet,
            ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        };

        settings.ValidationEventHandler += (_, args) => errors.Add(args.Message);

        try
        {
            using var stringReader = new StringReader(xmlContent);
            using var xmlReader = XmlReader.Create(stringReader, settings);
            var serializer = new XmlSerializer(typeof(ImportIssueDto));

            dto = serializer.Deserialize(xmlReader) as ImportIssueDto;
        }
        catch (InvalidOperationException ex)
        {
            errors.Add(ex.InnerException?.Message ?? ex.Message);
        }
        catch (XmlException ex)
        {
            errors.Add(ex.Message);
        }

        return errors;
    }

    private async Task<ImportResult> SaveAsync(
        ImportIssueDto? dto,
        CancellationToken cancellationToken)
    {
        if (dto is null)
        {
            return ImportResult.Failure("Import failed.", ["Request body could not be parsed."]);
        }

        var exists = await _issueRepository.ExistsByNumberAsync(dto.Number, cancellationToken);

        if (exists)
        {
            return ImportResult.Failure(
                "Import failed.",
                [$"Issue with number {dto.Number} already exists."]);
        }

        await _issueRepository.AddAsync(dto.ToEntity(), cancellationToken);

        return ImportResult.Success($"Issue #{dto.Number} imported.");
    }
}
