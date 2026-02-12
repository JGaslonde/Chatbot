using Xunit;
using FluentAssertions;
using Chatbot.API.Services;

namespace Chatbot.API.Tests.Services;

public class SentimentAnalysisServiceTests
{
    private readonly SimpleSentimentAnalysisService _service;

    public SentimentAnalysisServiceTests()
    {
        _service = new SimpleSentimentAnalysisService();
    }

    [Fact]
    public async Task AnalyzeSentiment_WithVeryPositiveText_ReturnsVeryPositiveSentiment()
    {
        // Arrange
        var message = "I love this! It's absolutely amazing and wonderful!";

        // Act
        var result = await _service.AnalyzeSentimentAsync(message);

        // Assert
        result.Sentiment.ToString().Should().Be("VeryPositive");
        result.Score.Should().BeGreaterThan(0.6);
    }

    [Fact]
    public async Task AnalyzeSentiment_WithVeryNegativeText_ReturnsVeryNegativeSentiment()
    {
        // Arrange
        var message = "I hate this! It's terrible, awful, and disgusting!";

        // Act
        var result = await _service.AnalyzeSentimentAsync(message);

        // Assert
        result.Sentiment.ToString().Should().Be("VeryNegative");
        result.Score.Should().BeLessThan(-0.6);
    }

    [Fact]
    public async Task AnalyzeSentiment_WithPositiveText_ReturnsPositiveSentiment()
    {
        // Arrange
        var message = "This is good and I'm happy with it.";

        // Act
        var result = await _service.AnalyzeSentimentAsync(message);

        // Assert
        result.Sentiment.ToString().Should().Be("Positive");
        result.Score.Should().BeInRange(0.2, 0.6);
    }

    [Fact]
    public async Task AnalyzeSentiment_WithNegativeText_ReturnsNegativeSentiment()
    {
        // Arrange
        var message = "This is bad and I'm sad about it.";

        // Act
        var result = await _service.AnalyzeSentimentAsync(message);

        // Assert
        result.Sentiment.ToString().Should().Be("Negative");
        result.Score.Should().BeInRange(-0.6, -0.2);
    }

    [Fact]
    public async Task AnalyzeSentiment_WithNeutralText_ReturnsNeutralSentiment()
    {
        // Arrange
        var message = "The sky is blue and grass is green.";

        // Act
        var result = await _service.AnalyzeSentimentAsync(message);

        // Assert
        result.Sentiment.ToString().Should().Be("Neutral");
        result.Score.Should().BeInRange(-0.2, 0.2);
    }

    [Fact]
    public async Task AnalyzeSentiment_WithEmptyString_ReturnsNeutralSentiment()
    {
        // Arrange
        var message = "";

        // Act
        var result = await _service.AnalyzeSentimentAsync(message);

        // Assert
        result.Sentiment.ToString().Should().Be("Neutral");
        result.Score.Should().Be(0.0);
    }

    [Fact]
    public async Task AnalyzeSentiment_WithMixedSentiment_ReturnsBalancedScore()
    {
        // Arrange
        var message = "I love the design but hate the performance.";

        // Act
        var result = await _service.AnalyzeSentimentAsync(message);

        // Assert
        // Score should be closer to neutral due to mixed sentiment
        result.Score.Should().BeInRange(-0.5, 0.5);
    }
}
