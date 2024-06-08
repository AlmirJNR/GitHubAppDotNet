using GitHub.Repos.Item.Item.Pulls.Item.Merge;
using GitHubApp.Api.Constants;
using GitHubApp.Api.Extensions;

namespace GitHubApp.Api.Services;

using GitHub.Repos.Item.Item.Pulls.Item;

public class GitHubPullRequestsService
{
    private Task Merge(WithPull_numberItemRequestBuilder repositoryPullRequests)
    {
        return repositoryPullRequests.Merge.PutAsync(new MergePutRequestBody
        {
            MergeMethod = MergePutRequestBody_merge_method.Merge
        });
    }

    public Task HandleCommand(WithPull_numberItemRequestBuilder repositoryPullRequests, string command)
    {
        if (command.EqualsIgnoreCase(CommandsConstants.Merge))
            return Merge(repositoryPullRequests);
        throw new ArgumentOutOfRangeException(nameof(command), command, null);
    }
}