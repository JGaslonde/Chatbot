using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for enhanced user preferences.
/// </summary>
public interface IUserPreferencesEnhancedService
{
    Task<EnhancedUserPreferencesDto> GetEnhancedPreferencesAsync(int userId);
    Task<EnhancedUserPreferencesDto> UpdateEnhancedPreferencesAsync(int userId, EnhancedUserPreferencesRequest request);
}
