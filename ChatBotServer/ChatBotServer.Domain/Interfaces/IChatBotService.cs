using ChatBotServer.Domain.Entities;

namespace ChatBotServer.Domain.Interfaces;

public interface IChatBotService
{
    IAsyncEnumerable<string> GenerateStreamingResponseAsync(string userMessage, IEnumerable<Entities.ChatMessage> conversationHistory, CancellationToken cancellationToken = default);
}