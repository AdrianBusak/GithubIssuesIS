using GitHubIssuesIS.Client.Models;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GithubIssuesIS.API.Grpc;

namespace GitHubIssuesIS.Client.Services;

public sealed class WeatherClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly Weather.WeatherClient _client;

    public WeatherClient(HttpClient httpClient)
    {
        var baseAddress = httpClient.BaseAddress ??
            throw new InvalidOperationException("API base address is missing.");

        _channel = GrpcChannel.ForAddress(
            baseAddress,
            new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(
                    GrpcWebMode.GrpcWeb,
                    new HttpClientHandler())
            });

        _client = new Weather.WeatherClient(_channel);
    }

    public async Task<List<WeatherStationDto>> GetCurrentTemperatureAsync(
        string city,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetCurrentTemperatureAsync(
            new WeatherRequest
            {
                City = city
            },
            cancellationToken: cancellationToken);

        return response.Stations
            .Select(station => new WeatherStationDto
            {
                City = station.City,
                Temperature = station.Temperature
            })
            .ToList();
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}
