using System.ClientModel;
using Azure.AI.OpenAI;
using ChatBotServer.Domain.Configuration;
using ChatBotServer.Domain.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace ChatBotServer.Infrastructure.Services;

public class AzureAIChatBotService : IChatBotService
{
    private readonly ChatClient _chatClient;
    private readonly ChatBotServiceOptions _options;

    public AzureAIChatBotService(IOptions<ChatBotServiceOptions> options)
    {
        _options = options.Value;
        
        if (string.IsNullOrEmpty(_options.AzureAIEndpoint) || string.IsNullOrEmpty(_options.AzureAIKey) || string.IsNullOrEmpty(_options.ModelName))
        {
            throw new InvalidOperationException("Azure AI endpoint, key and model must be configured");
        }

        var endpoint = new Uri(_options.AzureAIEndpoint);
        var credential = new ApiKeyCredential(_options.AzureAIKey);
        var azureClient = new AzureOpenAIClient(endpoint, credential);
        _chatClient = azureClient.GetChatClient(_options.ModelName);
    }

    public async Task<string> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a helpful assistant."),
            new UserChatMessage(userMessage)
        };

        var completion = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        return completion.Value.Content[0].Text ?? string.Empty;
    }

    public async IAsyncEnumerable<string> GenerateStreamingResponseAsync(string userMessage, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a helpful assistant."),
            new UserChatMessage(userMessage)
        };

        var completionUpdates = _chatClient.CompleteChatStreamingAsync(messages, cancellationToken: cancellationToken);

        await foreach (var completionUpdate in completionUpdates)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            foreach (var contentPart in completionUpdate.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(contentPart.Text))
                {
                    yield return contentPart.Text;
                }
            }
        }
    }
}