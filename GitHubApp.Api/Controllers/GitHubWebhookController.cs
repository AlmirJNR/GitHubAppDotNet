using System.Text.Json.Nodes;
using GitHubApp.Api.Constants;
using GitHubApp.Api.Extensions;
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
    public async Task<IActionResult> Handler([FromBody] JsonObject eventPayloadArgs)
    {
        // List of event names can be found here https://docs.github.com/en/webhooks/webhook-events-and-payloads
        object? eventPayload = Request.Headers[RequestHeadersConstants.XGitHubEvent].ToString() switch
        {
            WebhookEventsConstants.PullRequest => eventPayloadArgs.OctokitDeserialize<PullRequestEventPayload>(),
            _ => null
        };
        if (eventPayload is null)
            return BadRequest();

        var activityPayload = eventPayload as ActivityPayload ?? (ActivityPayload)eventPayload;
        var gitHubClient = await _gitHubClientFactory.CreateClient(activityPayload.Installation.Id);
        return eventPayload switch
        {
            PullRequestEventPayload payload => await gitHubClient.Handle(payload),
            _ => Problem()
        };
    }
}