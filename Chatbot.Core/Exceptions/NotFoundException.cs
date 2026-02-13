namespace Chatbot.Core.Exceptions;

public class NotFoundException : ChatbotException
{
    public NotFoundException(string resourceType, object resourceId)
        : base($"{resourceType} with ID '{resourceId}' was not found", "NOT_FOUND", 404)
    {
    }

    public NotFoundException(string message)
        : base(message, "NOT_FOUND", 404)
    {
    }
}
