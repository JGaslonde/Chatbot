using System.Security.Claims;
using Chatbot.API.Exceptions;

namespace Chatbot.API.Infrastructure.Auth.Interfaces;

public interface IUserContextProvider
{
    int GetUserId();
    string GetUsername();
    bool TryGetUserId(out int userId);
}
