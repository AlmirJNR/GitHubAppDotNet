using GitHubApp.Api.Extensions;
using GitHubApp.Api.Factories;
using GitHubApp.Api.Interfaces;
using GitHubApp.Api.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureJsonWebTokenOptions();
builder.Services.AddMemoryCache(options => options.SizeLimit = 1024);
builder.Services.TryAddScoped<ICacheService, CacheService>();
builder.Services.TryAddScoped<GitHubClientFactory>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();