using Bizzkit.Sdk.CloudMail;
using Bizzkit.Sdk.Dam;
using Bizzkit.Sdk.EcommerceSearch;
using Bizzkit.Sdk.Iam;
using Bizzkit.Sdk.Pim;
using Bizzkit.Sdk.Search;
using Bizzkit.Sdk.Search.Authenticated;
using Bizzkit.Sdk.Search.Authenticated.Services;

namespace BizzkitTraining;

public class BizzkitTest(
    IIamClientFactory iamClientFactory,
    IDamClientFactory damClientFactory,
    IPimClientFactory pimClientFactory,
    IMailClientFactory mailClientFactory,
    ISearchAdministrationClientFactory searchAdministrationClientFactory,
    ISearchClientFactory searchClientFactory,
    IAuthenticatedSearchSigningService authenticatedSearchSigningService
)
{
    public async Task<string> IamTest()
    {
        var client = await iamClientFactory.CreateAuthenticatedClientAsync();
        try
        {
            var count = (await client.ListRolesAsync()).Count();
            return "IAM OK";
        }
        catch (Exception ex)
        {
            return "IAM Error: " + ex.Message;
        }
    }

    public async Task<string> DamTest()
    {
        var client = await damClientFactory.CreateAuthenticatedClientAsync();
        try
        {
            var ok = await client.Test_EchoAsync("DAM ok");
            return "DAM OK";
        }
        catch (Exception ex)
        {
            return "DAM Error: " + ex.Message;
        }
    }

    public async Task<string> PimTest()
    {
        var client = await pimClientFactory.CreateAuthenticatedClientAsync();
        try
        {
            var ok = await client.Test_GetAsync();
            return "PIM OK";
        }
        catch (Exception ex)
        {
            return "PIM Error: " + ex.Message;
        }
    }

    public async Task<string> MailTest()
    {
        var client = await mailClientFactory.CreateAuthenticatedClientAsync();
        try
        {
            var ok = await client.ListRolesAsync();
            return "MAIL OK";
        }
        catch (Exception ex)
        {
            return "MAIL Error: " + ex.Message;
        }
    }

    public async Task<string> EcsAdminTest()
    {
        var client = await searchAdministrationClientFactory.CreateAuthenticatedClientAsync();
        try
        {
            var ok = await client.GetVersionInformationAsync();
            return "ECS Admin OK";
        }
        catch (Exception ex)
        {
            return "ECS Admin Error: " + ex.Message;
        }
    }

    public async Task<string> EcsHostTest()
    {
        var client = await searchClientFactory.CreateUnauthenticatedClientAsync();
        try
        {
            var ok = await client.GetVersionInformationAsync();
            return "ECS Host OK";
        }
        catch (Exception ex)
        {
            return "ECS Host Error: " + ex.Message;
        }
    }

    public async Task CreateSigningKey(string? audienceUrl, string? segmentId, string? issuerUrl = "https://mywebshop.example.com/")
    {
        var client = await searchAdministrationClientFactory.CreateAuthenticatedClientAsync();
        var signingKeySettings = new SearchSigningKeySettingsModel
        {
            AudienceUrl = new Uri(audienceUrl!),
            IssuerUrl = new Uri(issuerUrl!)
        };
        await client.UpsertSigningKeySettingsAsync(segmentId, signingKeySettings);
    }

    public async Task<AuthenticatedSearchToken> GetAuthenticatedSearchToken(string segmentId, string[] scope, Dictionary<string, Bizzkit.Sdk.Search.FilterModel> filters, TimeSpan tokenTtl)
    {

        var allowedScopeIds = new HashSet<string>(scope);
        var authSearchModel = new AuthenticatedSearchModel(segmentId)
        {
            AllowedScopeIds = allowedScopeIds,
            Filters = filters
        };
        var tokenOpts = new AuthenticatedSearchOptions { TokenTtl = tokenTtl };
        var token = await authenticatedSearchSigningService.CreateTokenAsync(authSearchModel, tokenOpts);
        return token;
    }
}