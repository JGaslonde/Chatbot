namespace Chatbot.API.Services.Analysis.Interfaces;

public interface IIntentRecognitionService
{
    Task<(string Intent, double Confidence)> RecognizeIntentAsync(string text);
}
