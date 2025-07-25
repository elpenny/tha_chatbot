﻿using ChatBotServer.Application.Features.Chat.Commands;
using ChatBotServer.Application.Features.Chat.Queries;
using ChatBotServer.Application.Features.Chat.Results;
using ChatBotServer.Domain.Entities;
using ChatBotServer.Domain.Interfaces;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class GetChatResponseStreamingHandler(
    IChatBotService chatBotService,
    IChatConversationRepository conversationRepository,
    IChatMessageRepository messageRepository)
    : IRequestHandler<GetChatResponseStreamingCommand, StreamingChatResult>
{
    public async Task<StreamingChatResult> Handle(GetChatResponseStreamingCommand request, CancellationToken cancellationToken)
    {
        // Get or create conversation
        ChatConversation conversation;
        if (request.ConversationId.HasValue)
        {
            conversation = await conversationRepository.GetByIdAsync(request.ConversationId.Value)
                ?? throw new InvalidOperationException($"Conversation with ID {request.ConversationId} not found");
        }
        else
        {
            // Create a new conversation
            conversation = new ChatConversation
            {
                Title = request.Content.Length > 50 ? request.Content[..50] + "..." : request.Content,
                CreatedAt = DateTime.UtcNow
            };
            await conversationRepository.AddAsync(conversation);
            await conversationRepository.SaveChangesAsync();
        }

        // Save user message
        var userMessage = new ChatMessage
        {
            Content = request.Content,
            Role = MessageRole.User,
            ConversationId = conversation.Id,
            CreatedAt = DateTime.UtcNow
        };
        await messageRepository.AddAsync(userMessage);
        await messageRepository.SaveChangesAsync();

        // Update conversation timestamp
        conversation.UpdatedAt = DateTime.UtcNow;
        await conversationRepository.UpdateAsync(conversation);
        await conversationRepository.SaveChangesAsync();

        // Create a wrapper that collects the response and saves it
        return new StreamingChatResult
        {
            ConversationId = conversation.Id,
            ContentStream = CreatePersistentStreamingResponse(request.Content, conversation.Id, cancellationToken)
        };
    }

    private async IAsyncEnumerable<string> CreatePersistentStreamingResponse(
        string userMessage, 
        int conversationId, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Use a wrapper to handle the saving logic
        await foreach (var chunk in StreamWithPersistence(userMessage, conversationId, cancellationToken))
        {
            yield return chunk;
        }
    }

    private async IAsyncEnumerable<string> StreamWithPersistence(
        string userMessage, 
        int conversationId, 
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var responseContent = string.Empty;
        var wasCompleted = false;
        
        // Get conversation history for multi-turn context
        var conversation = await conversationRepository.GetConversationWithMessagesAsync(conversationId);
        var conversationHistory = conversation?.Messages?.OrderBy(m => m.CreatedAt).ToList() ?? new List<ChatMessage>();
        
        IAsyncEnumerable<string> streamSource = chatBotService.GenerateStreamingResponseAsync(userMessage, conversationHistory, cancellationToken);
        
        try
        {
            await foreach (var chunk in streamSource.WithCancellation(cancellationToken))
            {
                responseContent += chunk;
                yield return chunk;
            }
            wasCompleted = true;
        }
        finally
        {
            // Always save the response content (even if partial due to cancellation)
            await SaveBotResponse(responseContent, conversationId, wasCompleted);
        }
    }

    private async Task SaveBotResponse(string content, int conversationId, bool wasCompleted)
    {
        if (!string.IsNullOrEmpty(content))
        {
            var botMessage = new ChatMessage
            {
                Content = content + (wasCompleted ? "" : " [Response cancelled]"),
                Role = MessageRole.Assistant,
                ConversationId = conversationId,
                CreatedAt = DateTime.UtcNow
            };
            await messageRepository.AddAsync(botMessage);
            await messageRepository.SaveChangesAsync();

            // Update conversation timestamp
            var conversation = await conversationRepository.GetByIdAsync(conversationId);
            if (conversation != null)
            {
                conversation.UpdatedAt = DateTime.UtcNow;
                await conversationRepository.UpdateAsync(conversation);
                await conversationRepository.SaveChangesAsync();
            }
        }
    }
}