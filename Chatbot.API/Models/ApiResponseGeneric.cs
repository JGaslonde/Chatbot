namespace Chatbot.API.Models;

/// <summary>
/// Generic API response wrapper for returning typed data
/// </summary>
public record ApiResponse<T>(
    bool Success,
    string? Message,
    T? Data,
    ApiError? Error
)
{
    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new(true, message ?? "Request successful", data, null);

    public static ApiResponse<T> FailedError(string message, string? errorCode = null, int? statusCode = null) =>
        new(false, message, default, new ApiError(message, errorCode, statusCode));

    public static ApiResponse<T> NotFound(string resource) =>
        new(false, $"{resource} not found", default, new ApiError($"{resource} not found", "NOT_FOUND", 404));

    public static ApiResponse<T> Unauthorized() =>
        new(false, "Unauthorized access", default, new ApiError("Unauthorized", "UNAUTHORIZED", 401));

    public static ApiResponse<T> Forbidden() =>
        new(false, "Access forbidden", default, new ApiError("Forbidden", "FORBIDDEN", 403));

    public static ApiResponse<T> BadRequest(string message) =>
        new(false, message, default, new ApiError(message, "BAD_REQUEST", 400));

    public static ApiResponse<T> InternalError(string message = "An internal error occurred") =>
        new(false, message, default, new ApiError(message, "INTERNAL_ERROR", 500));
}
