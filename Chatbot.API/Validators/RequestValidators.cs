using Chatbot.Core.Models;
using FluentValidation;

namespace Chatbot.API.Validators;

public class ChatMessageRequestValidator : AbstractValidator<ChatMessageRequest>
{
    public ChatMessageRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty")
            .MaximumLength(5000).WithMessage("Message cannot exceed 5000 characters")
            .Must(BeValidContent).WithMessage("Message contains invalid characters or patterns");
    }

    private bool BeValidContent(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        // Check for excessive special characters
        var specialCharCount = message.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        var ratio = (double)specialCharCount / message.Length;

        return ratio <= 0.7; // Allow up to 70% special characters
    }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(12).WithMessage("Password must be at least 12 characters long")
            .MaximumLength(128).WithMessage("Password cannot exceed 128 characters")
            .Must(HaveValidPasswordComplexity).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")
            .Must(p => !CommonPasswords.Contains(p.ToLowerInvariant())).WithMessage("This password is too common. Please choose a more unique password");
    }

    private bool HaveValidPasswordComplexity(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    private static readonly HashSet<string> CommonPasswords =
    [
        "password", "password1", "password123", "password1234", "password12345",
        "123456789", "1234567890", "12345678", "987654321",
        "qwertyuiop", "qwerty123", "qwerty1234", "qwerty12345",
        "letmein123", "welcome123", "admin1234", "admin12345",
        "iloveyou1!", "sunshine1!", "princess1!", "dragon1234",
        "monkey1234", "master1234", "superman1!", "batman1234",
        "starwars1!", "football1!", "baseball1!", "soccer1234",
        "abc123456", "abc1234567", "abcdefgh1!", "abcdefghi1",
        "trustno1!", "passw0rd!!", "p@ssword12", "p@ssw0rd12",
        "1qaz2wsx!!", "qazwsx1234", "zxcvbnm123",
        "summer2024!", "winter2024!", "spring2024!", "autumn2024!",
        "changeme12", "changeme1!", "default1234", "temp12345!"
    ];
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
    }
}

public class StartConversationRequestValidator : AbstractValidator<StartConversationRequest>
{
    public StartConversationRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));
    }
}
