namespace Chatbot.API.Exceptions;

public class ChatbotException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    public ChatbotException(string message, string errorCode = "GENERAL_ERROR", int statusCode = 500)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public ChatbotException(string message, Exception innerException, string errorCode = "GENERAL_ERROR", int statusCode = 500)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}
