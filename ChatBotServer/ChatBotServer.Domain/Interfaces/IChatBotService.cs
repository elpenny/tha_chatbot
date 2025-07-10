namespace ChatBotServer.Domain.Interfaces;

public interface IChatBotService
{
    Task<string> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GenerateStreamingResponseAsync(string userMessage, CancellationToken cancellationToken = default);
}