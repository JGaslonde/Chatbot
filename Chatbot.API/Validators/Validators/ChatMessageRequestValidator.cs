using Chatbot.Core.Models.Requests;
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
