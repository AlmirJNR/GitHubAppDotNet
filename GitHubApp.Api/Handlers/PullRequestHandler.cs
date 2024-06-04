using System.Text.RegularExpressions;
using GitHubApp.Api.Constants;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace GitHubApp.Api.Handlers;

public static partial class PullRequestHandler
{
    public static async Task<IActionResult> Handle(this GitHubClient client, PullRequestEventPayload payload)
    {
        if (payload.PullRequest.Draft)
            return new OkResult();

        if (ValidPullRequestTitleRegex().IsMatch(payload.PullRequest.Title))
            return new OkResult();

        var comments = await client.Issue.Comment.GetAllForIssue(payload.Repository.Id, payload.Number);
        if (comments.Any(x => x.Body.Contains(MessagesConstants.ErrorMessage)))
            return new OkResult();

        // In GitHub API terms, a pull request is actually a special type of issue
        // ref: https://github.com/octokit/octokit.net/issues/1862#issuecomment-418290045
        _ = await client.Issue.Comment.Create(payload.Repository.Id, payload.Number, MessagesConstants.ErrorMessage);
        _ = await client.Issue.Labels.AddToIssue(
            payload.Repository.Id,
            payload.Number,
            [LabelsConstants.InvalidPullRequest]
        );

        return new OkResult();
    }

    // Regex for string that has at least this in the start:
    // [XX-1] Lorem Ipsum
    [GeneratedRegex(@"\[[A-Za-z]{2,6}\-[0-9]{1,10}\] [A-Za-z]{2,}")]
    private static partial Regex ValidPullRequestTitleRegex();
}