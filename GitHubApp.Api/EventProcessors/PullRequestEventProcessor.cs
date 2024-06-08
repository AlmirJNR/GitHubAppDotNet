using System.Text.RegularExpressions;
using GitHubApp.Api.Constants;
using GitHubApp.Api.Factories;
using GitHubApp.Api.Services;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;

namespace GitHubApp.Api.EventProcessors;

public sealed partial class PullRequestEventProcessor : WebhookEventProcessor
{
    private readonly GitHubClientFactory _gitHubClientFactory;
    private readonly GitHubLabelsService _gitHubLabelsService;
    private readonly GitHubIssuesService _gitHubIssuesService;

    public PullRequestEventProcessor(
        GitHubClientFactory gitHubClientFactory,
        GitHubLabelsService gitHubLabelsService,
        GitHubIssuesService gitHubIssuesService
    )
    {
        _gitHubClientFactory = gitHubClientFactory;
        _gitHubLabelsService = gitHubLabelsService;
        _gitHubIssuesService = gitHubIssuesService;
    }

    // In GitHub API terms, a pull request is actually a special type of issue
    // ref: https://github.com/octokit/octokit.net/issues/1862#issuecomment-418290045
    protected override async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders __,
        PullRequestEvent pullRequestEvent,
        PullRequestAction ___
    )
    {
        if (pullRequestEvent is { Installation: null }
            or { PullRequest.Draft: true }
            or { PullRequest.ClosedAt: not null })
            return;

        var client = _gitHubClientFactory.GetOrCreateClient(pullRequestEvent.Installation.Id);
        var repository = client.Repos[pullRequestEvent.Repository?.Owner.Login][pullRequestEvent.Repository?.Name];
        var repositoryIssues = repository.Issues[Convert.ToInt32(pullRequestEvent.Number)];
        var labelsNames = pullRequestEvent.PullRequest.Labels.Select(x => x.Name).ToArray();

        if (ValidPullRequestTitleRegex().IsMatch(pullRequestEvent.PullRequest.Title))
        {
            if (!labelsNames.Contains(LabelsConstants.InvalidPullRequest))
                return;

            await _gitHubIssuesService.RemoveLabels(repositoryIssues, labelsNames, LabelsConstants.InvalidPullRequest);
            await _gitHubIssuesService.CreateComment(repositoryIssues, MessagesConstants.PullRequestTitleFixed);
            return;
        }

        if (labelsNames.Contains(LabelsConstants.InvalidPullRequest))
            return;

        var invalidPullRequestLabel = await _gitHubLabelsService.GetOrCreateLabel(
            repository,
            LabelsConstants.InvalidPullRequest,
            LabelsConstants.InvalidPullRequestColor
        );

        await _gitHubIssuesService.AddLabels(repositoryIssues, labelsNames, invalidPullRequestLabel.Name!);
        await _gitHubIssuesService.CreateComment(repositoryIssues, MessagesConstants.PullRequestTitleError);
    }

    // Regex for string that has at least this in the start:
    // [XX-1] Lorem Ipsum
    [GeneratedRegex(@"\[[A-Za-z]{2,6}\-[0-9]{1,10}\] [A-Za-z]{2,}")]
    private static partial Regex ValidPullRequestTitleRegex();
}