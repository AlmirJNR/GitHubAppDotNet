using GitHubApp.Api.EventProcessors;
using GitHubApp.Api.Extensions;
using GitHubApp.Api.Factories;
using GitHubApp.Api.Pools;
using GitHubApp.Api.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureJsonWebTokenOptions();
builder.Services.TryAddSingleton<GitHubClientPool>();
builder.Services.TryAddSingleton<GitHubClientFactory>();
builder.Services.TryAddSingleton<GitHubLabelsPool>();
builder.Services.TryAddScoped<GitHubLabelsService>();
builder.Services.TryAddScoped<GitHubIssuesService>();
builder.Services.TryAddScoped<GitHubPullRequestsService>();
builder.Services.TryAddScoped<WebhookEventProcessor, GitHubWebhookEventProcessor>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.MapGitHubWebhooks();
app.Run();