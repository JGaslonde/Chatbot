using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Core;

public interface ILlmResponseService
{
    Task<string?> GenerateResponseAsync(string userMessage, IEnumerable<Message> conversationHistory);
}

public class AnthropicLlmResponseService : ILlmResponseService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AnthropicLlmResponseService> _logger;

    public AnthropicLlmResponseService(IConfiguration configuration, ILogger<AnthropicLlmResponseService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> GenerateResponseAsync(string userMessage, IEnumerable<Message> conversationHistory)
    {
        var apiKey = _configuration["Anthropic:ApiKey"]
            ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Anthropic API key is not configured. Falling back to template responses.");
            return null;
        }

        try
        {
            var model = _configuration["Llm:Model"] ?? "claude-sonnet-4-6";
            var maxContextMessages = _configuration.GetValue<int>("Llm:MaxContextMessages", 20);

            var client = new AnthropicClient(new APIAuthentication(apiKey));

            // Build message history for context (most recent N messages)
            var historyMessages = conversationHistory
                .OrderBy(m => m.SentAt)
                .TakeLast(maxContextMessages)
                .Select(m => new Anthropic.SDK.Messaging.Message
                {
                    Role = m.Sender == MessageSender.User ? RoleType.User : RoleType.Assistant,
                    Content = [new TextContent { Text = m.Content }]
                })
                .ToList();

            // Ensure the conversation ends with the current user message
            if (historyMessages.LastOrDefault()?.Role != RoleType.User)
            {
                historyMessages.Add(new Anthropic.SDK.Messaging.Message
                {
                    Role = RoleType.User,
                    Content = [new TextContent { Text = userMessage }]
                });
            }

            var request = new MessageParameters
            {
                Model = model,
                MaxTokens = 1024,
                System = [new SystemMessage("You are a helpful, friendly chatbot assistant. Be concise and clear in your responses. " +
                                            "If you don't know something, say so honestly rather than guessing.")],
                Messages = historyMessages
            };

            var response = await client.Messages.GetClaudeMessageAsync(request);
            var text = response.Content.OfType<TextContent>().FirstOrDefault()?.Text;

            _logger.LogInformation("LLM response generated using model {Model}", model);
            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate LLM response");
            return null;
        }
    }
}
