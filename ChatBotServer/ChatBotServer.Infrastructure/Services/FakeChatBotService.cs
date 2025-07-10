using ChatBotServer.Domain.Interfaces;

namespace ChatBotServer.Infrastructure.Services;

public class FakeChatBotService : IChatBotService
{
    private static readonly string[] StaticResponses = new[]
    {
        "Hello! How can I help you today?",
        "That's an interesting question, let me think about it.",
        "I understand what you're asking about.",
        "Thank you for sharing that with me.",
        "I'm here to assist you with any questions you might have.",
        "That sounds like something worth exploring further.",
        "I appreciate you taking the time to ask me that.",
        "Let me provide you with a helpful response.",
        "I'm processing your request and will respond shortly.",
        "That's a great point you've brought up."
    };

    public Task<string> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        var random = new Random();
        var response = StaticResponses[random.Next(StaticResponses.Length)];
        return Task.FromResult(response);
    }

    public async IAsyncEnumerable<string> GenerateStreamingResponseAsync(string userMessage, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GenerateResponseAsync(userMessage, cancellationToken);
        
        // Stream character by character
        for (int i = 0; i < response.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return response[i].ToString();
            
            // Simulate typing delay
            await Task.Delay(50, cancellationToken);
        }
    }
}