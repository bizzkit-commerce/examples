using Bizzkit.Sdk.CloudMail;
using Bizzkit.Sdk.Dam;
using Bizzkit.Sdk.EcommerceSearch;
using Bizzkit.Sdk.Iam;
using Bizzkit.Sdk.Pim;
using Bizzkit.Sdk.Search;

namespace Bizzkit.Examples.Common;

public class DomainConfig
{
    public string? Authority { get; set; }
    public string? DamApiUrl { get; set; }
    public string? IamApiUrl { get; set; }
    public string? PimApiUrl { get; set; }
    public string? CmsApiUrl { get; set; }
    public string? EcsAdminApiUrl { get; set; }
    public string? EcsHostApiUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? MailApiUrl { get; set; }
    public string? Scope { get; private set; }

    public IamConnectionOptions GetIamConnectionOptions()
    {
        return new IamConnectionOptions
        {
            Authority = Authority,
            BaseUrl = IamApiUrl,
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };
    }

    public DamConnectionOptions GetDamConnectionOptions()
    {
        return new DamConnectionOptions
        {
            Authority = Authority,
            BaseUrl = DamApiUrl,
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };
    }

    public PimConnectionOptions GetPimConnectionOptions()
    {
        return new PimConnectionOptions
        {
            Authority = Authority,
            BaseUrl = PimApiUrl,
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };
    }

    public MailConnectionOptions GetMailConnectionOptions()
    {
        return new MailConnectionOptions
        {
            Authority = Authority,
            BaseUrl = MailApiUrl,
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };
    }

    public SearchAdministrationConnectionOptions GetSearchAdminConnectionOptions()
    {
        return new SearchAdministrationConnectionOptions
        {
            Authority = Authority,
            BaseUrl = EcsAdminApiUrl,
            ClientId = ClientId,
            ClientSecret = ClientSecret
        };
    }

    public SearchConnectionOptions GetSearchClientConnectionOptions()
    {
        return new SearchConnectionOptions
        {
            BaseUrl = EcsHostApiUrl
        };
    }

    public static DomainConfig GetConfigForLocalhost()
    {
        return new DomainConfig
        {
            Authority = "https://localhost:8001",
            DamApiUrl = "https://localhost:8012",
            IamApiUrl = "https://localhost:8002",
            PimApiUrl = "https://localhost:8006",
            CmsApiUrl = "https://localhost:8018",
            MailApiUrl = "https://localhost:8030",
            EcsAdminApiUrl = "https://localhost:8020",
            EcsHostApiUrl = "https://localhost:8021",
            ClientId = "admin",
            ClientSecret = "secretAdmin",
            Scope = "iam/, damapi/, cmsapi/, pimapi/, searchapi/, courierapi/, mailapi/",
        };
    }

}