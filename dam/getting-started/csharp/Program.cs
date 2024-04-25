using Bizzkit.Examples.Common;
using Bizzkit.Sdk.Dam;
using Microsoft.Extensions.DependencyInjection;

var config = DomainConfig.GetConfigForLocalhost();

// DI
var services = new ServiceCollection();
services.AddLogging();
services.AddSingleton<IDamClientFactory>(new DamClientFactory(config.GetDamConnectionOptions()));
var provider = services.BuildServiceProvider();

// Get SDK client
var factory = provider.GetRequiredService<IDamClientFactory>();
var client = await factory.CreateAuthenticatedClientAsync();

// Test
var testRespose = await client.Test_EchoAsync("DAM OK");
Console.WriteLine(testRespose);