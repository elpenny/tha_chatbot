namespace ChatBotServer.Domain.Configuration;

public class ChatBotServiceOptions
{
    public bool UseAzureAI { get; set; } = false;
    public string? AzureAIEndpoint { get; set; }
    public string? AzureAIKey { get; set; }
    public string? ModelName { get; set; }
}