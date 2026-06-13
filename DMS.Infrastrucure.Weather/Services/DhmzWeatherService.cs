using System.Globalization;
using System.Xml.Linq;
using GithubIssuesIS.Application.DTOs.Weather;
using GithubIssuesIS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace DMS.Infrastrucure.Weather.Services;

public sealed class DhmzWeatherService(
    HttpClient httpClient,
    ILogger<DhmzWeatherService> logger) : IDhmzWeatherService
{
    private const string DhmzXmlUrl = "https://vrijeme.hr/hrvatska_n.xml";

    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<DhmzWeatherService> _logger = logger;

    public async Task<IReadOnlyList<WeatherStationDto>> GetByCityAsync(
        string citySearchTerm,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(citySearchTerm))
        {
            throw new ArgumentException(
                "Naziv grada ili dio naziva grada je obavezan.",
                nameof(citySearchTerm));
        }

        try
        {
            using var response = await _httpClient.GetAsync(
                DhmzXmlUrl,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var document = await XDocument.LoadAsync(
                stream,
                LoadOptions.None,
                cancellationToken);

            var searchTerm = citySearchTerm.Trim();

            return document
                .Descendants("Grad")
                .Select(ParseStation)
                .Where(station =>
                    station is not null &&
                    station.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Cast<WeatherStationDto>()
                .OrderBy(station => station.City)
                .ToList();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(
                exception,
                "DHMZ XML request failed.");

            throw new InvalidOperationException(
                "DHMZ podaci trenutačno nisu dostupni.",
                exception);
        }
        catch (Exception exception) when (
            exception is not ArgumentException &&
            exception is not InvalidOperationException)
        {
            _logger.LogError(
                exception,
                "DHMZ XML processing failed.");

            throw new InvalidOperationException(
                "DHMZ podaci nisu mogli biti obrađeni.",
                exception);
        }
    }

    private static WeatherStationDto? ParseStation(XElement cityElement)
    {
        var cityName = cityElement
            .Element("GradIme")
            ?.Value
            .Trim();

        var temperature = cityElement
            .Element("Podatci")
            ?.Element("Temp")
            ?.Value
            .Trim();

        if (string.IsNullOrWhiteSpace(cityName) ||
            string.IsNullOrWhiteSpace(temperature))
        {
            return null;
        }

        return new WeatherStationDto
        {
            City = cityName,
            Temperature = FormatTemperature(temperature)
        };
    }

    private static string FormatTemperature(string value)
    {
        if (double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var temperature))
        {
            return $"{temperature:0.0} °C";
        }

        return $"{value} °C";
    }
}
