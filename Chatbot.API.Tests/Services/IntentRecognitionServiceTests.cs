using Xunit;
using FluentAssertions;
using Chatbot.API.Services;

namespace Chatbot.API.Tests.Services;

public class IntentRecognitionServiceTests
{
    private readonly SimpleIntentRecognitionService _service;

    public IntentRecognitionServiceTests()
    {
        _service = new SimpleIntentRecognitionService();
    }

    [Theory]
    [InlineData("Hello", "greeting")]
    [InlineData("Hi there", "greeting")]
    [InlineData("Good morning", "greeting")]
    [InlineData("Hey!", "greeting")]
    public async Task RecognizeIntent_WithGreeting_ReturnsGreetingIntent(string message, string expectedIntent)
    {
        // Act
        var result = await _service.RecognizeIntentAsync(message);

        // Assert
        result.Intent.Should().Be(expectedIntent);
        result.Confidence.Should().BeGreaterThan(0.0);
    }

    [Theory]
    [InlineData("Goodbye", "farewell")]
    [InlineData("Bye", "farewell")]
    [InlineData("See you later", "farewell")]
    public async Task RecognizeIntent_WithFarewell_ReturnsFarewellIntent(string message, string expectedIntent)
    {
        // Act
        var result = await _service.RecognizeIntentAsync(message);

        // Assert
        result.Intent.Should().Be(expectedIntent);
        result.Confidence.Should().BeGreaterThan(0.0);
    }

    [Theory]
    [InlineData("Help me", "help")]
    [InlineData("I need assistance", "help")]
    [InlineData("Can you help?", "help")]
    public async Task RecognizeIntent_WithHelpRequest_ReturnsHelpIntent(string message, string expectedIntent)
    {
        // Act
        var result = await _service.RecognizeIntentAsync(message);

        // Assert
        result.Intent.Should().Be(expectedIntent);
        result.Confidence.Should().BeGreaterThan(0.0);
    }

    [Theory]
    [InlineData("What is this?", "question")]
    [InlineData("Why does this happen?", "question")]
    [InlineData("When will this be ready?", "question")]
    public async Task RecognizeIntent_WithQuestion_ReturnsQuestionIntent(string message, string expectedIntent)
    {
        // Act
        var result = await _service.RecognizeIntentAsync(message);

        // Assert
        result.Intent.Should().Be(expectedIntent);
        result.Confidence.Should().BeGreaterThan(0.0);
    }

    [Theory]
    [InlineData("Do this for me", "command")]
    [InlineData("Create a new document", "command")]
    [InlineData("Start the process", "command")]
    public async Task RecognizeIntent_WithCommand_ReturnsCommandIntent(string message, string expectedIntent)
    {
        // Act
        var result = await _service.RecognizeIntentAsync(message);

        // Assert
        result.Intent.Should().Be(expectedIntent);
        result.Confidence.Should().BeGreaterThan(0.0);
    }

    [Fact]
    public async Task RecognizeIntent_WithUnknownText_ReturnsUnknownIntent()
    {
        // Arrange
        var message = "Random text with no clear intent pattern xyz123";

        // Act
        var result = await _service.RecognizeIntentAsync(message);

        // Assert
        result.Intent.Should().Be("unknown");
        result.Confidence.Should().Be(0.0);
    }

    [Fact]
    public async Task RecognizeIntent_WithEmptyString_ReturnsUnknownIntent()
    {
        // Arrange
        var message = "";

        // Act
        var result = await _service.RecognizeIntentAsync(message);

        // Assert
        result.Intent.Should().Be("unknown");
        result.Confidence.Should().Be(0.0);
    }
}
