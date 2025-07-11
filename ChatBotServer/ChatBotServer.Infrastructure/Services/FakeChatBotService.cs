using ChatBotServer.Domain.Interfaces;
using NLipsum.Core;

namespace ChatBotServer.Infrastructure.Services;

public class FakeChatBotService : IChatBotService
{
    private readonly LipsumGenerator _lipsumGenerator = new();
    private readonly Random _random = new();

    public async IAsyncEnumerable<string> GenerateStreamingResponseAsync(string userMessage, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GenerateResponseAsync(userMessage, cancellationToken);
        
        // Stream 1-5 characters at once with a variable delay
        int i = 0;
        while (i < response.Length)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            // Output 1-5 characters at once
            int charsToOutput = Math.Min(_random.Next(1, 6), response.Length - i);
            yield return response.Substring(i, charsToOutput);
            i += charsToOutput;
            
            // Variable typing delay between 20-80ms
            await Task.Delay(_random.Next(20, 81), cancellationToken);
        }
    }
    
    private Task<string> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        // Generate variable response lengths:
        // 30% short (1-2 sentences), 50% medium (3-5 sentences), 20% long (2-3 paragraphs)
        var responseType = _random.Next(1, 101);
        
        string response = responseType switch
        {
            <= 30 => string.Join(" ", _lipsumGenerator.GenerateSentences(_random.Next(1, 3))), // 1-2 sentences
            <= 80 => string.Join(" ", _lipsumGenerator.GenerateSentences(_random.Next(3, 6))), // 3-5 sentences
            _ => string.Join("\n\n", _lipsumGenerator.GenerateParagraphs(_random.Next(2, 4)))  // 2-3 paragraphs
        };
        
        return Task.FromResult(response);
    }
}