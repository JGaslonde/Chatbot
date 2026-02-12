using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chatbot.API.Infrastructure;
using Chatbot.Core.Models;

namespace Chatbot.API.Controllers;

/// <summary>
/// Base controller providing common functionality for all API controllers.
/// Implements DRY principle - Eliminates repeated patterns across controllers.
/// Single Responsibility - Focuses on user context and response formatting.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly IUserContextProvider UserContextProvider;
    protected readonly IApiResponseBuilder ResponseBuilder;
    protected readonly ILogger Logger;

    protected ApiControllerBase(
        IUserContextProvider userContextProvider,
        IApiResponseBuilder responseBuilder,
        ILogger logger)
    {
        UserContextProvider = userContextProvider;
        ResponseBuilder = responseBuilder;
        Logger = logger;
    }

    /// <summary>
    /// Gets the current user ID from claims. Throws UnauthorizedException if invalid.
    /// </summary>
    protected int CurrentUserId => UserContextProvider.GetUserId();

    /// <summary>
    /// Attempts to get user ID, returning false if unable to extract.
    /// </summary>
    protected bool TryGetUserId(out int userId) => UserContextProvider.TryGetUserId(out userId);

    /// <summary>
    /// Creates a success response with the given data.
    /// </summary>
    protected IActionResult Ok<T>(T data, string message = "Success")
    {
        return base.Ok(ResponseBuilder.Success(data, message));
    }

    /// <summary>
    /// Logs an action with structured information.
    /// </summary>
    protected void LogAction(string action, object? context = null)
    {
        Logger.LogInformation(
            "Action: {Action}, UserId: {UserId}, Context: {@Context}",
            action,
            TryGetUserId(out var userId) ? userId : -1,
            context);
    }
}
