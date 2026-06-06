using GitHubIssuesIS.Client;
using GitHubIssuesIS.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7279/")
});

builder.Services.AddScoped<IssueClient>();
builder.Services.AddScoped<AuthClient>();
builder.Services.AddScoped<ImportClient>();
builder.Services.AddScoped<SoapClient>();
builder.Services.AddScoped<WeatherClient>();
builder.Services.AddScoped<GraphQLClient>();

await builder.Build().RunAsync();
