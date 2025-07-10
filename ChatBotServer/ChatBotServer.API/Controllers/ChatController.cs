using Microsoft.AspNetCore.Mvc;

namespace ChatBotServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
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

    [HttpPost("message")]
    public async Task<IActionResult> GetNextMessage([FromBody] ChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        // Set up Server-Sent Events headers
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");
        Response.Headers.Add("Access-Control-Allow-Origin", "*");

        // Simple static response logic
        var random = new Random();
        var response = StaticResponses[random.Next(StaticResponses.Length)];

        try
        {
            // Stream the response character by character
            for (int i = 0; i < response.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var partialContent = response.Substring(0, i + 1);
                var sseData = new
                {
                    content = partialContent,
                    isComplete = i == response.Length - 1
                };

                await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(sseData)}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);

                // Simulate typing delay
                await Task.Delay(50, cancellationToken);
            }

            // Send final completion event
            var finalData = new { content = response, isComplete = true };
            await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(finalData)}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully
            var cancelData = new { content = response.Substring(0, Math.Min(response.Length, 10)), isComplete = false, cancelled = true };
            await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(cancelData)}\n\n", cancellationToken);
        }

        return new EmptyResult();
    }
}

public class ChatMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}

public class ChatMessageResponse
{
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}