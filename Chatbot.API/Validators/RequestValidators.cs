using Chatbot.API.Models.Requests;
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
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Must(HaveValidPasswordComplexity).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character");
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
