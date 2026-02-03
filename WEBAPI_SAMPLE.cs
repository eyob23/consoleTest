// SAMPLE: How to use AzureApplicationInsightsLogger in an ASP.NET Core Web API
// 
// This file demonstrates the recommended integration pattern for Web APIs.
// Copy this pattern to your own Web API project.

/*

=== Step 1: In your Program.cs ===

using AzureApplicationInsightsLogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Azure Application Insights logging
builder.Services.AddAzureApplicationInsightsLogging(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.ServiceName = builder.Configuration["ApplicationInsights:ServiceName"] ?? "WebAPI";
    options.ServiceVersion = "1.0.0";
    options.IncludeConsoleExporter = builder.Environment.IsDevelopment();
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


=== Step 2: In your appsettings.json ===

{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=your-id",
    "ServiceName": "MyWebAPI"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}


=== Step 3: In your Controller ===

using AzureApplicationInsightsLogger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ILogger<OrdersController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            // Your business logic here
            
            // Log the purchase event
            CustomEventLogger.LogPurchase(
                _logger,
                productId: request.ProductId,
                amount: request.Amount,
                currency: request.Currency ?? "USD"
            );
            
            return Ok(new { orderId = "ORD-123", success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{orderId}/user-action")]
    public IActionResult LogUserAction(string orderId, [FromBody] UserActionRequest request)
    {
        // Log custom user action
        CustomEventLogger.LogCustomAction(
            _logger,
            actionType: request.ActionType,
            userId: request.UserId,
            success: request.Success
        );
        
        return Ok();
    }
}

public class CreateOrderRequest
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
}

public class UserActionRequest
{
    public string ActionType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public bool Success { get; set; }
}

*/
