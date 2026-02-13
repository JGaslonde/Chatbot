namespace Chatbot.Core.Exceptions;

public class ForbiddenException : ChatbotException
{
    public ForbiddenException(string message = "Access forbidden")
        : base(message, "FORBIDDEN", 403)
    {
    }
}
