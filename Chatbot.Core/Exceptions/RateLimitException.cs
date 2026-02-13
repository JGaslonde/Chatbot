namespace Chatbot.Core.Exceptions;

public class RateLimitException : ChatbotException
{
    public int RetryAfterSeconds { get; }

    public RateLimitException(int retryAfterSeconds, string message = "Rate limit exceeded")
        : base(message, "RATE_LIMIT_EXCEEDED", 429)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}
