using System.Net;
using Bizzkit.Sdk.AIAssistant.Preview;

namespace MyFirstCustomFunction;

public sealed class InformationService(IAIAssistantClientFactory clientFactory)
{
    public async Task CreateIfMissingAsync(string informationSetId)
    {
        var client = await clientFactory.CreateAuthenticatedClientAsync();

        if (await ExistAsync(informationSetId)) return;

        await client.CreateCustomInformationSetAsync(new()
        {
            Id = informationSetId
        });
    }

    public async Task LoadAllFactsAsync(string informationSetId, string factsFolderName)
    {
        var client = await clientFactory.CreateAuthenticatedClientAsync();
        var allFacts = Directory.GetFiles(factsFolderName, "*.md");
        foreach (var fact in allFacts)
        {
            var factId = Path.GetFileNameWithoutExtension(fact);
            Console.WriteLine($"Learning about {fact}");
            var text = await File.ReadAllTextAsync(fact);
            await client.SaveCustomInformationSnippetAsync(informationSetId, factId, new()
            {
                Text = text
            });
        }
    }
    private async Task<bool> ExistAsync(string informationSetId)
    {
        try
        {
            var client = await clientFactory.CreateAuthenticatedClientAsync();
            var function = await client.GetCustomInformationSetAsync(informationSetId);

            return true;
        }
        catch (AIAssistantApiException exception) when (exception.StatusCode == (int)HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
