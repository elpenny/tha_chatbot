using ChatBotServer.Application.Features.Chat.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("message")]
    public async Task<IActionResult> GetNextMessage([FromBody] ChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        // Set up Server-Sent Events headers
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";
        Response.Headers["Access-Control-Allow-Origin"] = "*";

        try
        {
            var fullContent = string.Empty;
            
            // Get streaming response through MediatR
            var streamingQuery = new GetChatResponseStreamingQuery
            {
                Content = request.Content,
                ConversationId = request.ConversationId
            };
            
            var streamingResponse = await _mediator.Send(streamingQuery, cancellationToken);
            
            // Stream from MediatR handler
            await foreach (var chunk in streamingResponse)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                fullContent += chunk;
                var sseData = new
                {
                    content = fullContent,
                    isComplete = false
                };

                await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(sseData)}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            // Send final completion event
            var finalData = new { content = fullContent, isComplete = true };
            await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(finalData)}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully
            var cancelData = new { content = "", isComplete = false, cancelled = true };
            await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(cancelData)}\n\n", cancellationToken);
        }

        return new EmptyResult();
    }
}