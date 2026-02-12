using Xunit;
using FluentAssertions;
using Chatbot.API.Services;

namespace Chatbot.API.Tests.Services;

public class MessageFilterServiceTests
{
    private readonly MessageFilterService _service;

    public MessageFilterServiceTests()
    {
        _service = new MessageFilterService();
    }

    [Fact]
    public async Task FilterMessage_WithValidMessage_PassesFilter()
    {
        // Arrange
        var message = "This is a normal, clean message.";

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        result.IsClean.Should().BeTrue();
        result.Issues.Should().BeEmpty();
    }

    [Fact]
    public async Task FilterMessage_WithTooLongMessage_FailsFilter()
    {
        // Arrange
        var message = new string('a', 5001); // More than 5000 characters

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        result.IsClean.Should().BeFalse();
        result.Issues.Should().Contain(r => r.Contains("too long"));
    }

    [Fact]
    public async Task FilterMessage_WithExcessiveSpecialCharacters_FailsFilter()
    {
        // Arrange
        var message = "!@#$%^&*()!@#$%^&*()!@#$%^&*()"; // High ratio of special characters

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        result.IsClean.Should().BeFalse();
        result.Issues.Should().Contain(r => r.Contains("special characters"));
    }

    [Fact]
    public async Task FilterMessage_WithExcessiveRepetition_FailsFilter()
    {
        // Arrange
        var message = "Hellooooooo thereeeeee"; // Repeated characters

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        result.IsClean.Should().BeFalse();
        result.Issues.Should().Contain(r => r.Contains("repetition"));
    }

    [Fact]
    public async Task FilterMessage_WithEmptyMessage_FailsFilter()
    {
        // Arrange
        var message = "";

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        result.IsClean.Should().BeFalse();
        result.Issues.Should().Contain(r => r.Contains("empty"));
    }

    [Fact]
    public async Task FilterMessage_WithWhitespaceOnly_FailsFilter()
    {
        // Arrange
        var message = "   \t\n  ";

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        result.IsClean.Should().BeFalse();
        result.Issues.Should().Contain(r => r.Contains("empty"));
    }

    [Fact]
    public async Task FilterMessage_WithMultipleIssues_ReturnsAllReasons()
    {
        // Arrange
        var message = new string('!', 5001); // Too long + too many special chars

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        result.IsClean.Should().BeFalse();
        result.Issues.Count.Should().BeGreaterThan(1);
    }

    [Fact]
    public async Task FilterMessage_WithNormalLengthMessage_PassesLengthCheck()
    {
        // Arrange
        var message = new string('a', 4999); // Just under 5000

        // Act
        var result = await _service.FilterMessageAsync(message);

        // Assert
        // May fail other checks, but should not fail on length
        result.Issues.Should().NotContain(r => r.Contains("too long"));
    }
}
