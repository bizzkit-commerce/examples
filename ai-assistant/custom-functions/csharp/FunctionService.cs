using System.Net;
using Bizzkit.Sdk.AIAssistant.Preview;

namespace MyFirstCustomFunction;
public sealed class FunctionService(IAIAssistantClientFactory clientFactory)
{
    public async Task CreateOrUpdateFunctionAsync(string functionName, string function)
    {
        if (await FunctionExistAsync(functionName))
            await UpdateFunctionAsync(functionName, function);
        else
            await CreateFunctionAsync(functionName, function);
    }

    public async Task<string> CallFunctionAsync(string functionId, string question, string customInformationSetId)
    {
        var client = await clientFactory.CreateAuthenticatedClientAsync();
        //We can add any additional information needed to help the Model answer the question.
        //The parameters should match the ones used in the prompt.

        var parameters = new Dictionary<string, string>()
        {
            { "question", question },
            { "today", DateTimeOffset.Now.ToString("G") },
            { "name", "John Doe"},
            { "language", "English (United Kingdom)"},
            { "orders", "none"}
        };

        var retrievals = new RetrievalModel[]
        {
            new()
            {
                Handle = "facts",
                CustomInformationSetId = customInformationSetId,
                Phrase = question
            }
        };

        var request = new CustomFunctionCallRequestModel()
        {
            Parameters = parameters,
            Retrievals = retrievals,
            Mode = GenerationMode.Fast
        };

        var response = await client.CallAndWaitCustomFunctionAsync(functionId, request);

        return response.Result;
    }

    private async Task CreateFunctionAsync(string functionName, string function)
    {
        var client = await clientFactory.CreateAuthenticatedClientAsync();
        await client.CreateCustomFunctionAsync(new()
        {
            FunctionId = functionName,
            Text = function
        });
    }

    private async Task UpdateFunctionAsync(string functionName, string function)
    {
        var client = await clientFactory.CreateAuthenticatedClientAsync();
        await client.UpdateCustomFunctionAsync(functionName, new()
        {
            Text = function
        });
    }

    private async Task<bool> FunctionExistAsync(string functionName)
    {
        try
        {
            var client = await clientFactory.CreateAuthenticatedClientAsync();
            var function = await client.GetCustomFunctionAsync(functionName);

            return true;
        }
        catch (AIAssistantApiException exception) when (exception.StatusCode == (int)HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}