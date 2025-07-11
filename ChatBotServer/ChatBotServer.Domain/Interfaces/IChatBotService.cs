namespace ChatBotServer.Domain.Interfaces;

public interface IChatBotService
{
    IAsyncEnumerable<string> GenerateStreamingResponseAsync(string userMessage, CancellationToken cancellationToken = default);
}