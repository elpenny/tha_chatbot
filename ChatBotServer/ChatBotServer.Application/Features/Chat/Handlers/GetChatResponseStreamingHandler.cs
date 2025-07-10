using ChatBotServer.Application.Features.Chat.Queries;
using ChatBotServer.Domain.Entities;
using ChatBotServer.Domain.Interfaces;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class GetChatResponseStreamingHandler : IRequestHandler<GetChatResponseStreamingQuery, IAsyncEnumerable<string>>
{
    private readonly IChatBotService _chatBotService;
    private readonly IChatConversationRepository _conversationRepository;
    private readonly IChatMessageRepository _messageRepository;

    public GetChatResponseStreamingHandler(
        IChatBotService chatBotService,
        IChatConversationRepository conversationRepository,
        IChatMessageRepository messageRepository)
    {
        _chatBotService = chatBotService;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<IAsyncEnumerable<string>> Handle(GetChatResponseStreamingQuery request, CancellationToken cancellationToken)
    {
        // Get or create conversation
        ChatConversation conversation;
        if (request.ConversationId.HasValue)
        {
            conversation = await _conversationRepository.GetByIdAsync(request.ConversationId.Value)
                ?? throw new InvalidOperationException($"Conversation with ID {request.ConversationId} not found");
        }
        else
        {
            // Create new conversation
            conversation = new ChatConversation
            {
                Title = request.Content.Length > 50 ? request.Content[..50] + "..." : request.Content,
                CreatedAt = DateTime.UtcNow
            };
            await _conversationRepository.AddAsync(conversation);
            await _conversationRepository.SaveChangesAsync();
        }

        // Save user message
        var userMessage = new ChatMessage
        {
            Content = request.Content,
            Role = MessageRole.User,
            ConversationId = conversation.Id,
            CreatedAt = DateTime.UtcNow
        };
        await _messageRepository.AddAsync(userMessage);
        await _messageRepository.SaveChangesAsync();

        // Update conversation timestamp
        conversation.UpdatedAt = DateTime.UtcNow;
        await _conversationRepository.UpdateAsync(conversation);
        await _conversationRepository.SaveChangesAsync();

        // Create a wrapper that collects the response and saves it
        return CreatePersistentStreamingResponse(request.Content, conversation.Id, cancellationToken);
    }

    private async IAsyncEnumerable<string> CreatePersistentStreamingResponse(
        string userMessage, 
        int conversationId, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var responseContent = string.Empty;
        
        // Stream from AI service
        await foreach (var chunk in _chatBotService.GenerateStreamingResponseAsync(userMessage, cancellationToken))
        {
            responseContent += chunk;
            yield return chunk;
        }

        // Save bot response after streaming completes
        var botMessage = new ChatMessage
        {
            Content = responseContent,
            Role = MessageRole.Assistant,
            ConversationId = conversationId,
            CreatedAt = DateTime.UtcNow
        };
        await _messageRepository.AddAsync(botMessage);
        await _messageRepository.SaveChangesAsync();

        // Update conversation timestamp again
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation != null)
        {
            conversation.UpdatedAt = DateTime.UtcNow;
            await _conversationRepository.UpdateAsync(conversation);
            await _conversationRepository.SaveChangesAsync();
        }
    }
}