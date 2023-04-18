using GithubApp.Services;
using Microsoft.AspNetCore.Mvc;
using Octokit.Webhooks.Events;

namespace GithubApp.Controllers;

[ApiController]
[Route("api/webhook")]
public class GithubAppController
{
    private readonly IGithubAppService githubAppService;

    public GithubAppController(IGithubAppService githubAppService)
    {
        this.githubAppService = githubAppService;
    }

    [HttpPost]
    public async Task<string> Post(PushEvent pushEvent)
    {
        return await this.githubAppService.GithubWebhookEventAsync(pushEvent);
    }
}
