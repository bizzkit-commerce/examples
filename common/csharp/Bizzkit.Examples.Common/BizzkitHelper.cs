using Bizzkit.Sdk.CloudMail;
using Bizzkit.Sdk.Dam;
using Bizzkit.Sdk.EcommerceSearch;
using Bizzkit.Sdk.Iam;
using Bizzkit.Sdk.Pim;
using IdentityModel.Client;
using System.Reflection;
using System.Text;

namespace Bizzkit.Examples.Common
{
    public class BizzkitHelper
    {
        public static async Task<IIamClient> GetIamClientFactory(DomainConfig config)
        {
            var options = new IamConnectionOptions
            {
                Authority = config.Authority,
                BaseUrl = config.IamApiUrl,
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            };
            var factory = new IamClientFactory(options);
            return await factory.CreateAuthenticatedClientAsync().ConfigureAwait(false);
        }

        public static async Task<IPimClient> GetPimClientFactory(DomainConfig config)
        {
            var options = new PimConnectionOptions
            {
                Authority = config.Authority,
                BaseUrl = config.PimApiUrl,
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            };
            var factory = new PimClientFactory(options);
            return await factory.CreateAuthenticatedClientAsync().ConfigureAwait(false);
        }

        public static async Task<IMailClient> GetMailClientFactory(DomainConfig config)
        {
            var options = new MailConnectionOptions
            {
                Authority = config.Authority,
                BaseUrl = config.MailApiUrl,
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            };
            var factory = new MailClientFactory(options);
            return await factory.CreateAuthenticatedClientAsync().ConfigureAwait(false);
        }

        public static async Task<Bizzkit.Sdk.Search.ISearchClient> GetSearchClientFactory(HttpClient httpClient, DomainConfig config)
        {
            var options = new Bizzkit.Sdk.Search.SearchConnectionOptions
            {
                BaseUrl = config.EcsHostApiUrl
            };
            var factory = new Bizzkit.Sdk.Search.SearchClientFactory(options, httpClient);
            return await factory.CreateUnauthenticatedClientAsync();
        }

        public static async Task<ISearchAdministrationClient> GetSearchAdministrationClientFactory(DomainConfig config)
        {
            var options = new SearchAdministrationConnectionOptions
            {
                Authority = config.Authority,
                BaseUrl = config.EcsAdminApiUrl,
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            };
            var factory = new SearchAdministrationClientFactory(options);
            return await factory.CreateAuthenticatedClientAsync().ConfigureAwait(false);
        }

        public static async Task<ISearchAdministrationClient> GetSearchPreviewAdministrationClientFactory(DomainConfig config)
        {
            var options = new SearchAdministrationConnectionOptions
            {
                Authority = config.Authority,
                BaseUrl = config.EcsAdminApiUrl,
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            };
            var factory = new SearchAdministrationClientFactory(options);
            return await factory.CreateAuthenticatedClientAsync().ConfigureAwait(false);
        }

        public static async Task<IDamClient> GetDamClientFactory(DomainConfig config)
        {
            var options = new DamConnectionOptions
            {
                Authority = config.Authority,
                BaseUrl = config.DamApiUrl,
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            };
            var factory = new DamClientFactory(options);
            return await factory.CreateAuthenticatedClientAsync().ConfigureAwait(false);
        }

        public static async Task<string> GetToken()
        {
            var configuration = DomainConfig.GetConfigForLocalhost();
            var tokenEndpoint = new Uri($"{configuration.Authority}/connect/token");
            var tokenHttpClient = new HttpClient();
            var response = await tokenHttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest { Address = tokenEndpoint.ToString(), ClientId = configuration.ClientId, ClientSecret = configuration.ClientSecret, Scope = configuration.Scope }).ConfigureAwait(false);
            if (response.IsError) throw new IOException($"{response.ErrorType} error getting token from authority server at {tokenEndpoint}. If these settings look correct, double-check the configured clientsecret matches the corresponding setting in Auth. The following may contain additional information: '{response.ErrorDescription} {response.Error}'", response.Exception);
            return response.AccessToken;
        }

        public static async Task<List<MethodInfo>> GetEcsAdminMethods(DomainConfig config)
        {
            var adminClient = await GetSearchAdministrationClientFactory(config);
            var type = adminClient.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            return methods.ToList();
        }

        public static string GetMethodsAsString(List<MethodInfo> methods)
        {
            var stringBuilder = new StringBuilder();
            foreach (var method in methods)
            {
                // Use the custom method to get a better name for generic return types
                var returnType = GetBetterTypeName(method.ReturnType);

                var methodName = method.Name;

                var parameters = string.Join(", ", method.GetParameters().Select(p => $"{GetBetterTypeName(p.ParameterType)} {p.Name}"));

                var line = $"{returnType} {methodName}({parameters})";

                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString();
        }

        private static string GetBetterTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var genericTypeName = type.GetGenericTypeDefinition().Name;
            // Remove the `1, `2 etc. suffix to get the clean generic type name
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            // Get the generic arguments and format them
            var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetBetterTypeName));
            return $"{genericTypeName}<{genericArgs}>";
        }
    }
}