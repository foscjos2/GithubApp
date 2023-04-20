using Jose;
using Octokit;
using Octokit.Webhooks.Events;

namespace GithubApp.Services;

public class GithubAppService : IGithubAppService
{
    public GithubAppService() { }

    public async Task<string> GithubWebhookEventAsync(PushEvent pushEvent)
    {
        // determine which repo called us, generate an installation access token
        // (1) Grab the installationId
        long installationId = this.GetInstallationId(pushEvent);

        // (2) Generate our JWT
        Credentials creds = this.GenerateJwtCreds();

        var appClient = new GitHubClient(new ProductHeaderValue("MyApp")) { Credentials = creds };

        var token = await this.CreateInstallationAccessTokenAsync(appClient, installationId);

        return "Hello World";
    }

    private long GetInstallationId(PushEvent pushEvent)
    {
        long installationId;
        try
        {
            // The docs make it seem like we're not guaranteed to get an installationID here
            // if we don't find it here may need to ping the following endpoint;
            // https://docs.github.com/en/rest/apps/apps?apiVersion=2022-11-28#get-an-organization-installation-for-the-authenticated-app
            installationId = pushEvent.Installation.Id;
        }
        catch
        {
            throw new Exception(
                "There was an issue reading the installation ID - is this app configured correctly?"
            );
        }

        return installationId;
    }

    private Credentials GenerateJwtCreds()
    {
        // generate a JWT based on our private key and the app id - both of these can be grabbed from the github settings for this app
        var generator = new GitHubJwt.GitHubJwtFactory(
            new GitHubJwt.FilePrivateKeySource(
                "C:\\projects\\GithubApp\\GithubApp\\Secrets\\private.pem"
            ),
            new GitHubJwt.GitHubJwtFactoryOptions
            {
                AppIntegrationId = 320466,
                ExpirationSeconds = 500
            }
        );

        string jwtToken = generator.CreateEncodedJwtToken();

        return new Credentials(jwtToken, AuthenticationType.Bearer);
    }

    private async Task<AccessToken> CreateInstallationAccessTokenAsync(
        GitHubClient client,
        long installationId
    )
    {
        // this is where we talk to github and ask for an installation access token
        // we are able to use the installation Id to make sure we send this to the correct client repo
        // token will have whatever permissions we defined in our app (read medata and content at the time that I type this comment)
        // this only lasts for an hour, so don't stand around, make sure you use it right away!
        var result = await client.GitHubApps.CreateInstallationToken(installationId);

        return result;
    }
}

public interface IGithubAppService
{
    Task<string> GithubWebhookEventAsync(PushEvent pushEvent);
}
