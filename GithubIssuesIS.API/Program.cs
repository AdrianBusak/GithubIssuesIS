using GithubIssuesIS.API.Exstensions;
using GithubIssuesIS.Application.Interfaces;
using GithubIssuesIS.Repository;
using Microsoft.EntityFrameworkCore;

namespace GithubIssuesIS.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile(
                "appsettings.Local.json",
                optional: true,
                reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCoreServices(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(ServiceCollectionExtensions.ClientCorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<GithubIssuesIsDbContext>();
                await dbContext.Database.MigrateAsync();

                var authSeeder = scope.ServiceProvider.GetRequiredService<IAuthSeeder>();
                await authSeeder.SeedAsync();
            }

            app.Run();
        }
    }
}
