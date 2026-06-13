using DMS.Infrastrucure.Import.Services;
using GithubIssuesIS.Application.Import;
using Microsoft.Extensions.DependencyInjection;

namespace DMS.Infrastrucure.Import.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImportServices(this IServiceCollection services)
    {
        services.AddScoped<IImportService, ImportService>();

        return services;
    }
}
