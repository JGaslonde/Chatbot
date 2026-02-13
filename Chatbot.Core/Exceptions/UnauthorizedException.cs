namespace Chatbot.Core.Exceptions;

public class UnauthorizedException : ChatbotException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}
