using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Processing;

public interface IMessageFilterService
{
    Task<(bool IsClean, List<string> Issues)> FilterMessageAsync(string message);
}
