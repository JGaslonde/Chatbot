using Chatbot.Core.Models;

namespace Chatbot.API.Infrastructure.Http;

public interface IApiResponseBuilder
{
    ApiResponse<T> Success<T>(T data, string message = "Success");
    ApiResponse<T> Success<T>(T data, string message, int statusCode);
}
