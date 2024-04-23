using Bizzkit.Examples.Common;
using Bizzkit.Sdk.CloudMail;
using Bizzkit.Sdk.Dam;
using Bizzkit.Sdk.EcommerceSearch;
using Bizzkit.Sdk.Iam;
using Bizzkit.Sdk.Pim;
using Bizzkit.Sdk.Search;
using Bizzkit.Sdk.Search.Authenticated;
using BizzkitTraining;
using Microsoft.Extensions.DependencyInjection;


var config = DomainConfig.GetConfigForLocalhost();
var services = new ServiceCollection();
services.AddLogging();
services.AddSingleton<IIamClientFactory>(new IamClientFactory(config.GetIamConnectionOptions()));
services.AddSingleton<IDamClientFactory>(new DamClientFactory(config.GetDamConnectionOptions()));
services.AddSingleton<IPimClientFactory>(new PimClientFactory(config.GetPimConnectionOptions()));
services.AddSingleton<IMailClientFactory>(new MailClientFactory(config.GetMailConnectionOptions()));
services.AddSingleton<ISearchAdministrationClientFactory>(new SearchAdministrationClientFactory(config.GetSearchAdminConnectionOptions()));

HttpClient httpClient = new();
services.AddSingleton<ISearchClientFactory>(new SearchClientFactory(config.GetSearchClientConnectionOptions(), httpClient));
services.AddBizzkitAuthenticatedSearch();

services.AddSingleton<BizzkitTest>();
var provider = services.BuildServiceProvider();

var test = provider.GetService<BizzkitTest>();
if (test != null)
{
    Console.WriteLine(await test.IamTest());
    Console.WriteLine(await test.DamTest());
    Console.WriteLine(await test.PimTest());
    Console.WriteLine(await test.MailTest());
    Console.WriteLine(await test.EcsAdminTest());
    Console.WriteLine(await test.EcsHostTest());

    bool getAuthSearchToken = false;
    if (getAuthSearchToken)
    {
        const string segmentId = "merch-b2c-en";
        const string issuerUrl = "https://www.example.com";
        string[] allowedScopes = ["vip"];   // example
        var filters = new Dictionary<string, Bizzkit.Sdk.Search.FilterModel>
        {
            {
                "PriceGroup",
                new Bizzkit.Sdk.Search.FilterModel
                {
                    Values = new List<string> { "vip" } // example
                }
            }
        };
        await test.CreateSigningKey(config.EcsAdminApiUrl, segmentId, issuerUrl);
        var token = await test.GetAuthenticatedSearchToken(segmentId, allowedScopes, filters, TimeSpan.FromHours(1));
        Console.WriteLine("SearchToken:\r\n" + token.Token);
    }
}
