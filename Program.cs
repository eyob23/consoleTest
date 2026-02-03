// Example usage of the AzureApplicationInsightsLogger NuGet package
using AzureApplicationInsightsLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("=== Azure Application Insights Logger Package Demo ===\n");

// Example 1: Direct factory usage (for console apps)
Console.WriteLine("Example 1: Direct Factory Usage");
var config = new ApplicationInsightsLoggerConfiguration
{
    ConnectionString = "InstrumentationKey=5a1a0d4f-5d26-47e0-9125-a609eadfb1ff;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=28949414-c5b3-4491-9f57-97880bab1330",
    ServiceName = "ConsoleApp",
    ServiceVersion = "1.0.0",
    IncludeConsoleExporter = true
};

var loggerFactory = ApplicationInsightsLoggerFactory.CreateLoggerFactory(config);
var logger = loggerFactory.CreateLogger("DirectUsageExample");

// Log custom events
CustomEventLogger.LogUserLogin(logger, "user123", DateTime.UtcNow);
CustomEventLogger.LogPurchase(logger, "PROD-456", 99.99m, "USD");
CustomEventLogger.LogHighValuePurchase(logger, 99.99m, 50);

Console.WriteLine("✓ Events logged via direct factory usage\n");

// Example 2: Dependency Injection usage (for ASP.NET Core Web API)
Console.WriteLine("Example 2: Dependency Injection Usage (Web API Style)");
var services = new ServiceCollection();

// Register the logger with DI
services.AddAzureApplicationInsightsLogging(options =>
{
    options.ConnectionString = "InstrumentationKey=5a1a0d4f-5d26-47e0-9125-a609eadfb1ff;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=28949414-c5b3-4491-9f57-97880bab1330";
    options.ServiceName = "WebAPI";
    options.ServiceVersion = "2.0.0";
    options.IncludeConsoleExporter = true;
});

var serviceProvider = services.BuildServiceProvider();
var apiLogger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("WebAPIExample");

// Log custom events through the API logger
CustomEventLogger.LogCustomAction(apiLogger, "PageView", "user456", true);
var customProps = new Dictionary<string, object?>
{
    { "OrderId", "ORD-789" },
    { "TotalAmount", 299.99 },
    { "ItemCount", 3 }
};
CustomEventLogger.LogCustomEvent(apiLogger, "OrderPlaced", customProps);

Console.WriteLine("✓ Events logged via dependency injection\n");

// Flush and cleanup
Console.WriteLine("Waiting for telemetry export...");
System.Threading.Thread.Sleep(2000);

loggerFactory.Dispose();
serviceProvider.Dispose();

Console.WriteLine("\n=== Demo Complete ===");
Console.WriteLine("View custom events in Azure Portal:");
Console.WriteLine("Application Insights > Logs > customEvents");
Console.WriteLine("Query: customEvents | where name in ('UserLoginEvent', 'PurchaseEvent', 'HighValuePurchase', 'CustomUserAction', 'OrderPlaced')");
