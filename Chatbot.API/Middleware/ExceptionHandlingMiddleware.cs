using Chatbot.API.Exceptions;
using Chatbot.Core.Models;
using System.Text.Json;

namespace Chatbot.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        ApiResponse<object> apiResponse;
        
        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = validationEx.StatusCode;
                apiResponse = new ApiResponse<object>(
                    false,
                    validationEx.Message,
                    null,
                    validationEx.ValidationErrors);
                _logger.LogWarning(validationEx, "Validation error: {Message}", validationEx.Message);
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = notFoundEx.StatusCode;
                apiResponse = new ApiResponse<object>(
                    false,
                    notFoundEx.Message,
                    null);
                _logger.LogWarning(notFoundEx, "Resource not found: {Message}", notFoundEx.Message);
                break;

            case UnauthorizedException unauthorizedEx:
                response.StatusCode = unauthorizedEx.StatusCode;
                apiResponse = new ApiResponse<object>(
                    false,
                    unauthorizedEx.Message,
                    null);
                _logger.LogWarning(unauthorizedEx, "Unauthorized access: {Message}", unauthorizedEx.Message);
                break;

            case ForbiddenException forbiddenEx:
                response.StatusCode = forbiddenEx.StatusCode;
                apiResponse = new ApiResponse<object>(
                    false,
                    forbiddenEx.Message,
                    null);
                _logger.LogWarning(forbiddenEx, "Forbidden access: {Message}", forbiddenEx.Message);
                break;

            case ConflictException conflictEx:
                response.StatusCode = conflictEx.StatusCode;
                apiResponse = new ApiResponse<object>(
                    false,
                    conflictEx.Message,
                    null);
                _logger.LogWarning(conflictEx, "Conflict: {Message}", conflictEx.Message);
                break;

            case RateLimitException rateLimitEx:
                response.StatusCode = rateLimitEx.StatusCode;
                response.Headers["Retry-After"] = rateLimitEx.RetryAfterSeconds.ToString();
                apiResponse = new ApiResponse<object>(
                    false,
                    rateLimitEx.Message,
                    null);
                _logger.LogWarning(rateLimitEx, "Rate limit exceeded: {Message}", rateLimitEx.Message);
                break;

            case MessageFilteredException messageFilteredEx:
                response.StatusCode = messageFilteredEx.StatusCode;
                apiResponse = new ApiResponse<object>(
                    false,
                    messageFilteredEx.Message,
                    null,
                    messageFilteredEx.FilterReasons);
                _logger.LogWarning(messageFilteredEx, "Message filtered: {Message}", messageFilteredEx.Message);
                break;

            case ChatbotException chatbotEx:
                response.StatusCode = chatbotEx.StatusCode;
                apiResponse = new ApiResponse<object>(
                    false,
                    chatbotEx.Message,
                    null);
                _logger.LogError(chatbotEx, "Chatbot error: {Message}", chatbotEx.Message);
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse = new ApiResponse<object>(
                    false,
                    "An unexpected error occurred. Please try again later.",
                    null);
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var result = JsonSerializer.Serialize(apiResponse, jsonOptions);
        await response.WriteAsync(result);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
