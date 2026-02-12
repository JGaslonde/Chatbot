using Chatbot.Core.Models;

namespace Chatbot.API.Infrastructure.Http;

/// <summary>
/// Provides consistent API response formatting.
/// Applies DRY - Eliminates repeated ApiResponse<T> wrapping logic across controllers.
/// </summary>
public interface IApiResponseBuilder
{
    ApiResponse<T> Success<T>(T data, string message = "Success");
    ApiResponse<T> Success<T>(T data, string message, int statusCode);
}

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
