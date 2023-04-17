using Microsoft.AspNetCore.Mvc;

namespace GithubApp.Controllers;

[ApiController]
[Route("api/webhook")]
public class GithubAppController
{
    public GithubAppController() { }

    [HttpGet]
    public string Get()
    {
        return "Hello world";
    }
}
