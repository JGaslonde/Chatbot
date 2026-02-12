namespace Chatbot.API.Services;

public interface IMessageFilterService
{
    Task<(bool IsClean, List<string> Issues)> FilterMessageAsync(string message);
}

public class MessageFilterService : IMessageFilterService
{
    private readonly HashSet<string> _bannedWords = new()
    {
        // Add profanities and inappropriate words here
        "badword1", "badword2", "offensive"
    };

    private readonly HashSet<string> _spamPatterns = new()
    {
        "$$$", "click here", "buy now", "limited offer", "act now"
    };

    public async Task<(bool IsClean, List<string> Issues)> FilterMessageAsync(string message)
    {
        var issues = new List<string>();
        var lower = message.ToLower();

        // Check for banned words
        foreach (var bannedWord in _bannedWords)
        {
            if (lower.Contains(bannedWord))
            {
                issues.Add($"Contains inappropriate content: {bannedWord}");
            }
        }

        // Check for spam patterns
        foreach (var pattern in _spamPatterns)
        {
            if (lower.Contains(pattern))
            {
                issues.Add($"Possible spam detected: {pattern}");
            }
        }

        // Check message length
        if (message.Length > 5000)
        {
            issues.Add("Message exceeds maximum length (5000 characters)");
        }

        // Check for excessive special characters
        var specialCharCount = message.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        if (specialCharCount > message.Length * 0.3)
        {
            issues.Add("Message contains too many special characters");
        }

        // Check for repeated characters (spam indicator)
        if (System.Text.RegularExpressions.Regex.IsMatch(message, @"(.)\1{4,}"))
        {
            issues.Add("Message contains excessive character repetition");
        }

        return await Task.FromResult((issues.Count == 0, issues));
    }
}
