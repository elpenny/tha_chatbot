using System.Text.Json.Serialization;

namespace ChatBotServer.Application.Features.Chat.Results;

public class GetConversationListResult
{
    [JsonPropertyName("conversations")]
    public IEnumerable<ConversationSummary> Conversations { get; set; } = new List<ConversationSummary>();
}

public class ConversationSummary
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
    
    [JsonPropertyName("messageCount")]
    public int MessageCount { get; set; }
    
    [JsonPropertyName("lastMessage")]
    public string? LastMessage { get; set; }
}