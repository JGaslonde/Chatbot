using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Infrastructure.Http.Interfaces;

public interface IApiResponseBuilder
{
    ApiResponse<T> Success<T>(T data, string message = "Success");
    ApiResponse<T> Success<T>(T data, string message, int statusCode);
}
