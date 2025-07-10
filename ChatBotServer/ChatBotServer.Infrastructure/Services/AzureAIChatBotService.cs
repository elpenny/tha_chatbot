using Azure;
using Azure.AI.Inference;
using ChatBotServer.Domain.Configuration;
using ChatBotServer.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace ChatBotServer.Infrastructure.Services;

public class AzureAIChatBotService : IChatBotService
{
    private readonly ChatCompletionsClient _client;
    private readonly ChatBotServiceOptions _options;

    public AzureAIChatBotService(IOptions<ChatBotServiceOptions> options)
    {
        _options = options.Value;
        
        if (string.IsNullOrEmpty(_options.AzureAIEndpoint) || string.IsNullOrEmpty(_options.AzureAIKey) || string.IsNullOrEmpty(_options.ModelName))
        {
            throw new InvalidOperationException("Azure AI endpoint, key and model must be configured");
        }

        var endpoint = new Uri(_options.AzureAIEndpoint);
        var credential = new AzureKeyCredential(_options.AzureAIKey);
        _client = new ChatCompletionsClient(endpoint, credential);
    }

    public async Task<string> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        var requestOptions = new ChatCompletionsOptions
        {
            Messages = 
            {
                new ChatRequestSystemMessage("You are a helpful assistant."),
                new ChatRequestUserMessage(userMessage)
            },
            Model = _options.ModelName,
        };

        var response = await _client.CompleteAsync(requestOptions, cancellationToken);
        return response.Value.Content ?? string.Empty;
    }

    public async IAsyncEnumerable<string> GenerateStreamingResponseAsync(string userMessage, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var requestOptions = new ChatCompletionsOptions
        {
            Messages = 
            {
                new ChatRequestSystemMessage("You are a helpful assistant."),
                new ChatRequestUserMessage(userMessage)
            },
            Model = _options.ModelName,
        };

        var response = await _client.CompleteStreamingAsync(requestOptions, cancellationToken);
        var currentContent = string.Empty;

        await foreach (var chatUpdate in response.EnumerateValues())
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
            {
                currentContent += chatUpdate.ContentUpdate;
                yield return currentContent;
            }
        }
    }
}