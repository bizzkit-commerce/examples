using Bizzkit.Examples.Common;
using Bizzkit.Sdk.Dam;
using Microsoft.Extensions.DependencyInjection;

var config = DomainConfig.GetConfigForLocalhost();
var services = new ServiceCollection();

services.AddLogging();
services.AddSingleton<IDamClientFactory>(new DamClientFactory(config.GetDamConnectionOptions()));

var provider = services.BuildServiceProvider();
var factory = provider.GetRequiredService<IDamClientFactory>();
var client = await factory.CreateAuthenticatedClientAsync();

const string culture = "en-GB";
var myFileId = Guid.Parse("YOUR_ID_HERE"); // Change this to your uploaded file's ID.

// Retrieving a file
var cachedFileParams = new CachedFileInfoParameter
{
    FileId = myFileId,
    Transformation = CachedFileInfoParameterTransformation.ResizeToCanvas,
    AllowWhitespaceExpansion = false,
    AllowEnlarge = false,
    Height = 400,
    Width = 400,
};

var files = await client.Files_CachedDetailsAsync(culture, [cachedFileParams]);

var result = files.First();

if (result.Status == CachedFileDetailsResultStatus.Success)
{
    Console.WriteLine($"Cached file URL: {result.CachedFileDetails.FileUri}");
}
else
{
    Console.WriteLine($"Request failed with status: {result.Status}");
}
