namespace Chatbot.API.Exceptions;

public class ConflictException : ChatbotException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", 409)
    {
    }
}
