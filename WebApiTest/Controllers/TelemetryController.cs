using AzureApplicationInsightsLogger;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly ILogger<TelemetryController> _logger;

    public TelemetryController(ILogger<TelemetryController> logger)
    {
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult LogLogin([FromBody] LoginRequest request)
    {
        CustomEventLogger.LogUserLogin(_logger, request.UserId, DateTime.UtcNow);
        return Ok(new { message = "Login event sent" });
    }

    [HttpPost("purchase")]
    public IActionResult LogPurchase([FromBody] PurchaseRequest request)
    {
        CustomEventLogger.LogPurchase(_logger, request.ProductId, request.Amount, request.Currency);

        if (request.Amount >= request.HighValueThreshold)
        {
            CustomEventLogger.LogHighValuePurchase(_logger, request.Amount, request.HighValueThreshold);
        }

        return Ok(new { message = "Purchase event sent" });
    }

    [HttpPost("action")]
    public IActionResult LogAction([FromBody] CustomActionRequest request)
    {
        CustomEventLogger.LogCustomAction(_logger, request.ActionType, request.UserId, request.Success);
        return Ok(new { message = "Custom action event sent" });
    }
}

public sealed class LoginRequest
{
    public string UserId { get; set; } = string.Empty;
}

public sealed class PurchaseRequest
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal HighValueThreshold { get; set; } = 50m;
}

public sealed class CustomActionRequest
{
    public string ActionType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public bool Success { get; set; }
}
