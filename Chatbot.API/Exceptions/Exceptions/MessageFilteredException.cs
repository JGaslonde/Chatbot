namespace Chatbot.API.Exceptions;

public class MessageFilteredException : ChatbotException
{
    public List<string> FilterReasons { get; }

    public MessageFilteredException(List<string> reasons)
        : base("Message filtered due to content policy violations", "MESSAGE_FILTERED", 400)
    {
        FilterReasons = reasons;
    }
}
