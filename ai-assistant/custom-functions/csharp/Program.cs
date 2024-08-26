using System.Reflection;
using System.Text.Json;
using Bizzkit.Sdk.AIAssistant.Preview;
using MyFirstCustomFunction;

/*
 *  This is a simplified example of a request response interaction with a custom function.
 *  Notice that Information snippets are not immediately available and that their availability status can be fetch through the API.
 *  In most cases it would be recommended to call the custom function as long running operation instead of using call and wait as done in this example.
 */
Console.WriteLine("Loading settings.");
if (Assembly.GetEntryAssembly()?.Location is string path && Path.GetDirectoryName(path) is string dir)
    Directory.SetCurrentDirectory(dir);

var settingsPath = "settings.json";
var connectionsOptionsPath = "connection-options.json";
var functionPath = "prompt.md";
var facts = "facts";

var settings = JsonSerializer.Deserialize<Settings>(await File.ReadAllTextAsync(settingsPath));
var options = JsonSerializer.Deserialize<AIAssistantConnectionOptions>(await File.ReadAllTextAsync(connectionsOptionsPath));
var function = await File.ReadAllTextAsync(functionPath);

if (settings == null)
{
    Console.WriteLine($"Could not load settings from file {Path.GetFullPath(settingsPath)}");
    return;
}

if (options == null)
{
    Console.WriteLine($"Could not load connection options from file {Path.GetFullPath(connectionsOptionsPath)}");
    return;
}

if (string.IsNullOrEmpty(function))
{
    Console.WriteLine($"Could not function definition from  {Path.GetFullPath(functionPath)}");
    return;
}

var factory = new AIAssistantClientFactory(options);
var functionService = new FunctionService(factory);
Console.WriteLine("Creating function.");
await functionService.CreateOrUpdateFunctionAsync(settings.FunctionId, function);

if (settings.LoadCustomInformation)

    Console.WriteLine("Creating information set.");
{
    var informationService = new InformationService(factory);
    await informationService.CreateIfMissingAsync(settings.InformationSetId);
    await informationService.LoadAllFactsAsync(settings.InformationSetId, facts);
}
while (true)
{
    Console.WriteLine("What do you want to ask?");
    var question = Console.ReadLine();
    if (!string.IsNullOrEmpty(question))
    {
        var answer = await functionService.CallFunctionAsync(settings.FunctionId, question, settings.InformationSetId);
        Console.WriteLine(answer);
    }
}
