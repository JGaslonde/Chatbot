namespace Chatbot.API.Exceptions;

public class ValidationException : ChatbotException
{
    public List<string> ValidationErrors { get; }

    public ValidationException(string message, List<string>? errors = null)
        : base(message, "VALIDATION_ERROR", 400)
    {
        ValidationErrors = errors ?? new List<string>();
    }
}
