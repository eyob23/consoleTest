# AzureApplicationInsightsLogger NuGet Package

A reusable NuGet package for seamless integration of Azure Application Insights with custom event logging in ASP.NET Core Web APIs and console applications using OpenTelemetry.

## Features

- 🔧 Easy integration with ASP.NET Core Dependency Injection
- 📊 Pre-built custom event logging helpers
- 🎯 OpenTelemetry integration with Azure Monitor exporter
- 📝 Fully documented XML comments
- 🚀 Console app and Web API support
- 💾 Configured for .NET 8.0

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package AzureApplicationInsightsLogger
```

Or via Package Manager Console:

```powershell
Install-Package AzureApplicationInsightsLogger
```

## Quick Start

### 1. ASP.NET Core Web API (Recommended)

In your `Program.cs`:

```csharp
using AzureApplicationInsightsLogger;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Application Insights logging
builder.Services.AddAzureApplicationInsightsLogging(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.ServiceName = "MyWebAPI";
    options.ServiceVersion = "1.0.0";
});

var app = builder.Build();
app.Run();
```

Or with a direct connection string:

```csharp
builder.Services.AddAzureApplicationInsightsLogging(
    connectionString: "your-connection-string",
    serviceName: "MyWebAPI",
    serviceVersion: "1.0.0"
);
```

### 2. In Your Controller or Service

```csharp
using AzureApplicationInsightsLogger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public OrderController(ILogger<OrderController> logger, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    [HttpPost("purchase")]
    public IActionResult PlaceOrder(OrderRequest request)
    {
        // Log purchase event
        CustomEventLogger.LogPurchase(
            _logger,
            productId: request.ProductId,
            amount: request.Amount,
            currency: "USD"
        );

        return Ok(new { success = true });
    }

    [HttpPost("user-action")]
    public IActionResult LogAction(string actionType, string userId)
    {
        // Log custom action
        CustomEventLogger.LogCustomAction(_logger, actionType, userId, success: true);
        return Ok();
    }
}
```

### 3. Console Applications

```csharp
using AzureApplicationInsightsLogger;

var config = new ApplicationInsightsLoggerConfiguration
{
    ConnectionString = "your-connection-string",
    ServiceName = "MyConsoleApp",
    ServiceVersion = "1.0.0",
    IncludeConsoleExporter = true  // For debugging
};

var loggerFactory = ApplicationInsightsLoggerFactory.CreateLoggerFactory(config);
var logger = loggerFactory.CreateLogger("MyApp");

// Log events
CustomEventLogger.LogUserLogin(logger, "user123", DateTime.UtcNow);
CustomEventLogger.LogPurchase(logger, "PROD-456", 99.99m, "USD");

loggerFactory.Dispose();
```

## Available Methods

### Custom Event Logging Helpers

```csharp
// Log user login
CustomEventLogger.LogUserLogin(logger, userId, loginTime);

// Log purchase
CustomEventLogger.LogPurchase(logger, productId, amount, currency);

// Log high-value purchase warning
CustomEventLogger.LogHighValuePurchase(logger, amount, threshold);

// Log custom user action
CustomEventLogger.LogCustomAction(logger, actionType, userId, success);

// Log custom event with arbitrary properties
CustomEventLogger.LogCustomEvent(logger, eventName, properties);
```

## Configuration

### ApplicationInsightsLoggerConfiguration

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ConnectionString` | string | "" | Azure Application Insights connection string (required) |
| `ServiceName` | string | "DefaultService" | Service name for resource identification |
| `ServiceVersion` | string | "1.0.0" | Service version |
| `IncludeConsoleExporter` | bool | false | Include console exporter for debugging |

## Getting Your Connection String

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to your Application Insights resource
3. Copy the connection string from the overview page
4. Store it securely (e.g., Azure Key Vault, user secrets, or appsettings.json)

## Querying Events in Azure Portal

In Application Insights > Logs, use KQL queries:

```kusto
customEvents
| where timestamp > ago(24h)
| project name, tostring(customDimensions), timestamp
```

Or filter by specific events:

```kusto
customEvents
| where name in ('UserLoginEvent', 'PurchaseEvent', 'HighValuePurchase')
| summarize Count=count() by name
```

## Dependencies

- OpenTelemetry 1.12.0
- OpenTelemetry.Exporter.Console 1.12.0
- OpenTelemetry.Extensions.Hosting 1.12.0
- Azure.Monitor.OpenTelemetry.Exporter 1.4.0
- Microsoft.Extensions.DependencyInjection 8.0.0
- Microsoft.Extensions.Logging 8.0.0

## License

MIT

## Support

For issues or questions, please visit the repository or contact the maintainers.
