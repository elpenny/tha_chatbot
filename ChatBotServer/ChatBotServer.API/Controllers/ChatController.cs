using ChatBotServer.Application.Features.Chat.Commands;
using ChatBotServer.Application.Features.Chat.Queries;
using ChatBotServer.Application.Features.Chat.Requests;
using ChatBotServer.Application.Features.Chat.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Send a message to the chatbot and receive a streaming response
    /// </summary>
    /// <param name="request">The chat message request containing content and optional conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server-Sent Events stream with bot response</returns>
    /// <response code="200">Successful streaming response</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPost("message")]
    [Produces("text/event-stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNextMessage([FromBody] ChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        // Set up Server-Sent Events headers
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";

        var streamingResponse = default(StreamingChatResult);
        var fullContent = string.Empty;
        
        try
        {
            
            // Get streaming response through MediatR
            var streamingQuery = new GetChatResponseStreamingCommand
            {
                Content = request.Content,
                ConversationId = request.ConversationId
            };
            
            streamingResponse = await mediator.Send(streamingQuery, cancellationToken);
            
            // Stream from MediatR handler
            await foreach (var chunk in streamingResponse.ContentStream)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                fullContent += chunk;
                var sseData = new
                {
                    content = fullContent,
                    conversationId = streamingResponse.ConversationId,
                    isComplete = false
                };

                await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(sseData)}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            // Send final completion event
            var finalData = new { 
                content = fullContent, 
                conversationId = streamingResponse.ConversationId, 
                isComplete = true 
            };
            await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(finalData)}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully - the handler will still save partial content
            // Send final state with current content
            var cancelData = new { 
                content = fullContent, 
                conversationId = streamingResponse?.ConversationId ?? 0,
                isComplete = false, 
                cancelled = true 
            };
            
            try
            {
                await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(cancelData)}\n\n");
                await Response.Body.FlushAsync();
            }
            catch (OperationCanceledException)
            {
                // Response stream was already closed, which is fine
            }
        }

        return new EmptyResult();
    }

    /// <summary>
    /// Retrieve conversation history with all messages
    /// </summary>
    /// <param name="id">The conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conversation with messages and ratings</returns>
    /// <response code="200">Conversation found and returned</response>
    /// <response code="404">Conversation not found</response>
    [HttpGet("conversations/{id}")]
    [ProducesResponseType(typeof(GetConversationHistoryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversationHistory(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetConversationHistoryQuery { ConversationId = id };
            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Update the rating for a specific message
    /// </summary>
    /// <param name="id">The message ID</param>
    /// <param name="request">Rating request (1 for thumbs up, -1 for thumbs down, null to remove rating)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or error status</returns>
    /// <response code="200">Rating updated successfully</response>
    /// <response code="404">Message not found</response>
    /// <response code="400">Invalid rating value</response>
    [HttpPut("messages/{id}/rating")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMessageRating(int id, [FromBody] UpdateMessageRatingRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateMessageRatingCommand 
        { 
            MessageId = id, 
            Rating = request.Rating 
        };
        
        var success = await mediator.Send(command, cancellationToken);
        
        if (!success)
            return NotFound("Message not found");
            
        return Ok();
    }
}