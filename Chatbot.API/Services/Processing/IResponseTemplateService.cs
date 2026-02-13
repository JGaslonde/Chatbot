using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Processing;

public interface IResponseTemplateService
{
    string GenerateResponse(string userMessage, string intent, Sentiment sentiment, double sentimentScore);
    string GenerateContextAwareResponse(string userMessage, List<Message> recentMessages, string intent, Sentiment sentiment);
}
