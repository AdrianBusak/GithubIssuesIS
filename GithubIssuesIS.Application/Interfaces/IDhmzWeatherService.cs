using GithubIssuesIS.Application.DTOs.Weather;

namespace GithubIssuesIS.Application.Interfaces;

public interface IDhmzWeatherService
{
    Task<IReadOnlyList<WeatherStationDto>> GetByCityAsync(
        string citySearchTerm,
        CancellationToken cancellationToken = default);
}
