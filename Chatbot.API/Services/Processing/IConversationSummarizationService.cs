using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Processing;

public interface IConversationSummarizationService
{
    string GenerateSummary(List<Message> messages);
    string GenerateTitle(List<Message> messages);
}
