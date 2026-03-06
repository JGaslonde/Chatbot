using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IUserPreferencesManagementService
{
    Task<EnhancedUserPreferencesDto?> GetPreferencesAsync();
    Task<EnhancedUserPreferencesDto?> UpdatePreferencesAsync(EnhancedUserPreferencesRequest request);
}
