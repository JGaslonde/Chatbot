using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Processing.Interfaces;

public interface IConversationSummarizationService
{
    string GenerateSummary(List<Message> messages);
    string GenerateTitle(List<Message> messages);
}
