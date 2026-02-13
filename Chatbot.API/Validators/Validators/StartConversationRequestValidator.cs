using Chatbot.Core.Models.Requests;
using FluentValidation;

namespace Chatbot.API.Validators;

public class StartConversationRequestValidator : AbstractValidator<StartConversationRequest>
{
    public StartConversationRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));
    }
}
