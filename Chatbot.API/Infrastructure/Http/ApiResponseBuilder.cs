using Chatbot.Core.Models.Responses;
using Chatbot.API.Infrastructure.Http.Interfaces;

namespace Chatbot.API.Infrastructure.Http;

public class ApiResponseBuilder : IApiResponseBuilder
{
    public ApiResponse<T> Success<T>(T data, string message = "Success")
    {
        return new ApiResponse<T>(true, message, data);
    }

    public ApiResponse<T> Success<T>(T data, string message, int statusCode)
    {
        return new ApiResponse<T>(true, message, data);
    }
}
