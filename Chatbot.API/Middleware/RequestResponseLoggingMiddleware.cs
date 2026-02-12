using System.Diagnostics;
using System.Text;

namespace Chatbot.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        // Log request
        await LogRequest(context, requestId);

        // Capture original response body stream
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
            stopwatch.Stop();

            // Log response
            await LogResponse(context, requestId, stopwatch.ElapsedMilliseconds);

            // Copy response to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Request {RequestId} failed after {ElapsedMs}ms: {Message}",
                requestId, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context, string requestId)
    {
        var request = context.Request;

        var logMessage = new StringBuilder();
        logMessage.AppendLine($"REQUEST {requestId}:");
        logMessage.AppendLine($"  Method: {request.Method}");
        logMessage.AppendLine($"  Path: {request.Path}");
        logMessage.AppendLine($"  QueryString: {request.QueryString}");
        logMessage.AppendLine($"  IP: {context.Connection.RemoteIpAddress}");

        // Log relevant headers (excluding sensitive ones)
        if (request.Headers.Any())
        {
            logMessage.AppendLine("  Headers:");
            foreach (var (key, value) in request.Headers)
            {
                if (!IsSensitiveHeader(key))
                {
                    logMessage.AppendLine($"    {key}: {value}");
                }
            }
        }

        // Log request body for POST/PUT requests (with size limit)
        if ((request.Method == "POST" || request.Method == "PUT") && 
            request.ContentLength > 0 && 
            request.ContentLength < 10000) // Only log if less than 10KB
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
            var bodyText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0; // Reset for next middleware
            
            // Mask sensitive data in body
            bodyText = MaskSensitiveData(bodyText);
            logMessage.AppendLine($"  Body: {bodyText}");
        }

        _logger.LogInformation(logMessage.ToString());
    }

    private async Task LogResponse(HttpContext context, string requestId, long elapsedMs)
    {
        var response = context.Response;

        var logMessage = new StringBuilder();
        logMessage.AppendLine($"RESPONSE {requestId}:");
        logMessage.AppendLine($"  Status: {response.StatusCode}");
        logMessage.AppendLine($"  Duration: {elapsedMs}ms");

        // Log relevant response headers
        if (response.Headers.Any())
        {
            logMessage.AppendLine("  Headers:");
            foreach (var (key, value) in response.Headers)
            {
                if (!IsSensitiveHeader(key))
                {
                    logMessage.AppendLine($"    {key}: {value}");
                }
            }
        }

        // Log response body (with size limit)
        if (response.Body.CanSeek && response.Body.Length > 0 && response.Body.Length < 10000)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            
            // Mask sensitive data
            bodyText = MaskSensitiveData(bodyText);
            logMessage.AppendLine($"  Body: {bodyText}");
        }

        var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        _logger.Log(logLevel, logMessage.ToString());
    }

    private bool IsSensitiveHeader(string headerName)
    {
        var sensitiveHeaders = new[]
        {
            "Authorization",
            "Cookie",
            "Set-Cookie",
            "X-API-Key",
            "X-Auth-Token"
        };

        return sensitiveHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
    }

    private string MaskSensitiveData(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Simple masking for common sensitive fields
        var sensitivePatterns = new[]
        {
            ("password", "\"password\"\\s*:\\s*\"[^\"]+\"", "\"password\":\"***\""),
            ("token", "\"token\"\\s*:\\s*\"[^\"]+\"", "\"token\":\"***\""),
            ("apiKey", "\"apiKey\"\\s*:\\s*\"[^\"]+\"", "\"apiKey\":\"***\""),
            ("passwordHash", "\"passwordHash\"\\s*:\\s*\"[^\"]+\"", "\"passwordHash\":\"***\"")
        };

        foreach (var (_, pattern, replacement) in sensitivePatterns)
        {
            text = System.Text.RegularExpressions.Regex.Replace(
                text, pattern, replacement, 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return text;
    }
}

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
