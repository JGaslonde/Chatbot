namespace Chatbot.API.Services.Analysis;

public interface IIntentRecognitionService
{
    Task<(string Intent, double Confidence)> RecognizeIntentAsync(string text);
}
