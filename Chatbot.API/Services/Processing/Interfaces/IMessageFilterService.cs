using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Processing.Interfaces;

public interface IMessageFilterService
{
    Task<(bool IsClean, List<string> Issues)> FilterMessageAsync(string message);
}
