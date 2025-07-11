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

    public AzureAIChatBotService(IOptions<ChatBotServiceOptions> options)
    {
        var configuration = options.Value;
        
        if (string.IsNullOrEmpty(configuration.AzureAIEndpoint) || string.IsNullOrEmpty(configuration.AzureAIKey) || string.IsNullOrEmpty(configuration.ModelName))
        {
            throw new InvalidOperationException("Azure AI endpoint, key and model must be configured");
        }

        var endpoint = new Uri(configuration.AzureAIEndpoint);
        var credential = new ApiKeyCredential(configuration.AzureAIKey);
        var azureClient = new AzureOpenAIClient(endpoint, credential);
        _chatClient = azureClient.GetChatClient(configuration.ModelName);
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