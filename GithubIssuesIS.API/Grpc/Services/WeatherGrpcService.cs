using GithubIssuesIS.Application.Interfaces;
using Grpc.Core;

namespace GithubIssuesIS.API.Grpc.Services;

public sealed class WeatherGrpcService(
    IDhmzWeatherService weatherService,
    ILogger<WeatherGrpcService> logger) : Weather.WeatherBase
{
    private readonly IDhmzWeatherService _weatherService = weatherService;
    private readonly ILogger<WeatherGrpcService> _logger = logger;

    public override async Task<WeatherResponse> GetCurrentTemperature(
        WeatherRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.City))
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Naziv grada ili dio naziva grada je obavezan."));
        }

        try
        {
            var stations = await _weatherService.GetByCityAsync(
                request.City,
                context.CancellationToken);

            var response = new WeatherResponse();
            response.Stations.AddRange(stations.Select(station => new WeatherStation
            {
                City = station.City,
                Temperature = station.Temperature
            }));

            return response;
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(
                exception,
                "DHMZ weather service failed for search term {City}.",
                request.City);

            throw new RpcException(
                new Status(
                    StatusCode.Unavailable,
                    exception.Message));
        }
    }
}
