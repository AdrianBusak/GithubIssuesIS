using DMS.Infrastrucure.Weather.Services;
using GithubIssuesIS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DMS.Infrastrucure.Weather.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDhmzWeatherServices(
        this IServiceCollection services)
    {
        services.AddHttpClient<IDhmzWeatherService, DhmzWeatherService>();

        return services;
    }
}
