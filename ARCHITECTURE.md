# Package Architecture

## Structure Overview

```
AzureApplicationInsightsLogger (NuGet Package)
│
├── ApplicationInsightsLogger.cs
│   ├── ApplicationInsightsLoggerConfiguration (class)
│   │   ├── ConnectionString (property)
│   │   ├── ServiceName (property)
│   │   ├── ServiceVersion (property)
│   │   └── IncludeConsoleExporter (property)
│   │
│   ├── ApplicationInsightsLoggerFactory (static class)
│   │   ├── CreateLoggerFactory() (method)
│   │   └── CreateLogger() (method)
│   │
│   └── CustomEventLogger (static class)
│       ├── LogUserLogin()
│       ├── LogPurchase()
│       ├── LogHighValuePurchase()
│       ├── LogCustomAction()
│       └── LogCustomEvent()
│
└── ServiceCollectionExtensions.cs
    ├── AddAzureApplicationInsightsLogging() (overload 1)
    └── AddAzureApplicationInsightsLogging() (overload 2)


```

## Usage Patterns

### Pattern 1: ASP.NET Core Web API (Recommended)

```
Web API Application
    │
    ├── Program.cs
    │   └── builder.Services.AddAzureApplicationInsightsLogging()
    │
    ├── Controllers/
    │   └── OrderController.cs
    │       ├── ILogger<OrderController> _logger (injected)
    │       └── CustomEventLogger.LogPurchase(_logger, ...)
    │
    └── appsettings.json
        └── ApplicationInsights:ConnectionString
```

### Pattern 2: Console Application

```
Console Application
    │
    ├── Program.cs
    │   ├── new ApplicationInsightsLoggerConfiguration()
    │   ├── ApplicationInsightsLoggerFactory.CreateLoggerFactory()
    │   └── CustomEventLogger.Log*()
    │
    └── App runs
        └── Telemetry sent to Azure
```

## Data Flow

```
Application Code
        │
        ├─► CustomEventLogger.LogPurchase()
        │   └─► ILogger.LogInformation()
        │
        ├─► Configured LoggerFactory
        │   ├─► OpenTelemetry Logger Provider
        │   │   └─► Console Exporter (optional)
        │   │
        │   └─► Azure Monitor Log Exporter
        │       └─► Azure Application Insights
        │           └─► customEvents table in Azure Portal
        │
        └─► KQL Queries
            └─► Dashboards & Alerts
```

## Integration Points

### 1. Configuration
```
appsettings.json
    ↓
builder.Configuration["ApplicationInsights:ConnectionString"]
    ↓
ApplicationInsightsLoggerConfiguration
    ↓
ApplicationInsightsLoggerFactory
```

### 2. Dependency Injection
```
IServiceCollection
    ↓
AddAzureApplicationInsightsLogging()
    ↓
ILoggerFactory (singleton)
    ↓
Constructor Injection
    ↓
Service/Controller
```

### 3. Event Logging
```
CustomEventLogger.Log*()
    ↓
ILogger.LogInformation/LogWarning/etc.
    ↓
OpenTelemetry Processor
    ↓
Azure Monitor Exporter
    ↓
Azure Application Insights
```

## Class Relationships

```
                    ┌────────────────────┐
                    │ ILoggerFactory     │
                    └────────────────────┘
                              △
                              │
                              │ implements
                              │
          ┌───────────────────┼───────────────────┐
          │                   │                   │
          ▼                   ▼                   ▼
    ┌──────────┐       ┌──────────┐       ┌──────────────┐
    │ Console  │       │ Debug    │       │ OpenTelemetry│
    │ Logger   │       │ Logger   │       │   Logger     │
    └──────────┘       └──────────┘       └──────────────┘
                                                  │
                                                  ▼
                                         ┌────────────────┐
                                         │ Azure Monitor  │
                                         │    Exporter    │
                                         └────────────────┘
                                                  │
                                                  ▼
                                    ┌──────────────────────────┐
                                    │ Azure Application        │
                                    │ Insights Instance        │
                                    └──────────────────────────┘
```

## Supported Scenarios

### ✓ Web API Controllers
```csharp
[HttpPost]
public IActionResult Purchase([FromBody] OrderRequest req)
{
    CustomEventLogger.LogPurchase(_logger, req.ProductId, req.Amount, "USD");
    return Ok();
}
```

### ✓ Background Services
```csharp
public class OrderProcessingService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        CustomEventLogger.LogCustomAction(_logger, "OrderProcessing", "service", true);
    }
}
```

### ✓ Middleware
```csharp
app.Use(async (context, next) =>
{
    CustomEventLogger.LogCustomAction(_logger, "RequestReceived", context.User?.Identity?.Name, true);
    await next();
});
```

### ✓ Hosted Services
```csharp
public class TelemetryService : IHostedService
{
    private readonly ILoggerFactory _loggerFactory;
    
    public TelemetryService(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
}
```

## Configuration Hierarchy

```
Appsettings.json
├─ ApplicationInsights
│  ├─ ConnectionString (required)
│  └─ ServiceName (optional: defaults to "WebAPI")
│
Programmatic Configuration
├─ ServiceName
├─ ServiceVersion
├─ ConnectionString
└─ IncludeConsoleExporter

Environment Variables
└─ Can override appsettings values
```

## Event Types Supported

```
User Events
├─ UserLoginEvent
│  └─ userId, loginTime
│
Business Events
├─ PurchaseEvent
│  └─ productId, amount, currency
├─ HighValuePurchase
│  └─ amount, threshold
│
Action Events
├─ CustomUserAction
│  └─ actionType, userId, success
│
Generic Events
└─ CustomEvent
   └─ eventName, Dictionary<string, object>
```

## Package Dependencies

```
AzureApplicationInsightsLogger
├─ OpenTelemetry 1.12.0
├─ OpenTelemetry.Exporter.Console 1.12.0
├─ OpenTelemetry.Extensions.Hosting 1.12.0
├─ Azure.Monitor.OpenTelemetry.Exporter 1.4.0
├─ Microsoft.Extensions.DependencyInjection 8.0.0
└─ Microsoft.Extensions.Logging 8.0.0
```

## Publishing Workflow

```
Source Code
    ↓
dotnet pack -c Release
    ↓
AzureApplicationInsightsLogger.1.0.0.nupkg
    ↓
├─ NuGet.org
│  └─ Public Feed
│
├─ Azure Artifacts
│  └─ Private Feed
│
└─ Local Feed
   └─ Testing Only
    ↓
dotnet add package AzureApplicationInsightsLogger
    ↓
Consumer Application
    ↓
Uses Package in Production
```

---

This architecture provides a clean, extensible foundation for Azure Application Insights integration across your organization's .NET applications.
