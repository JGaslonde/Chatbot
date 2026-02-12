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

public class ValidationException : ChatbotException
{
    public List<string> ValidationErrors { get; }

    public ValidationException(string message, List<string>? errors = null)
        : base(message, "VALIDATION_ERROR", 400)
    {
        ValidationErrors = errors ?? new List<string>();
    }
}

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

public class UnauthorizedException : ChatbotException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}

public class ForbiddenException : ChatbotException
{
    public ForbiddenException(string message = "Access forbidden")
        : base(message, "FORBIDDEN", 403)
    {
    }
}

public class ConflictException : ChatbotException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", 409)
    {
    }
}

public class RateLimitException : ChatbotException
{
    public int RetryAfterSeconds { get; }

    public RateLimitException(int retryAfterSeconds, string message = "Rate limit exceeded")
        : base(message, "RATE_LIMIT_EXCEEDED", 429)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}

public class MessageFilteredException : ChatbotException
{
    public List<string> FilterReasons { get; }

    public MessageFilteredException(List<string> reasons)
        : base("Message filtered due to content policy violations", "MESSAGE_FILTERED", 400)
    {
        FilterReasons = reasons;
    }
}
