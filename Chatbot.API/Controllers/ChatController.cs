using Microsoft.AspNetCore.Mvc;

namespace Chatbot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    [HttpPost("send")]
    public IActionResult SendMessage([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty");
        }

        // TODO: Integrate with chatbot logic
        var response = new ChatResponse(
            Message: $"Echo: {request.Message}",
            Timestamp: DateTime.UtcNow
        );

        return Ok(response);
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy" });
    }
}

public record ChatRequest(string Message);

public record ChatResponse(string Message, DateTime Timestamp);
