namespace Chatbot.API.Infrastructure.Authorization;

public interface IConversationAccessControl
{
    Task<bool> HasAccessAsync(int conversationId, int userId);
    Task VerifyAccessAsync(int conversationId, int userId);
}
