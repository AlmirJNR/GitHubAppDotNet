using System.Text.Json;
using System.Text.Json.Nodes;
using GitHubApp.Api.Constants;
using GitHubApp.Api.Factories;
using GitHubApp.Api.Handlers;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace GitHubApp.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GitHubWebhookController : ControllerBase
{
    private readonly GitHubClientFactory _gitHubClientFactory;

    public GitHubWebhookController(GitHubClientFactory gitHubClientFactory)
    {
        _gitHubClientFactory = gitHubClientFactory;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> Handler(JsonObject jsonObject)
    {
        object? obj = Request.Headers[RequestHeadersConstants.XGitHubEvent].ToString() switch
        {
            // List of event names can be found here https://docs.github.com/en/webhooks/webhook-events-and-payloads
            WebhookEventsConstants.PullRequest => jsonObject.Deserialize<PullRequestEventPayload>(),
            _ => null
        };

        if (obj is null)
            return Task.FromResult<IActionResult>(BadRequest());

        var gitHubClient = _gitHubClientFactory.CreateClient();
        return obj switch
        {
            PullRequestEventPayload payload => gitHubClient.Handle(payload),
            _ => Task.FromResult<IActionResult>(Problem())
        };
    }
}