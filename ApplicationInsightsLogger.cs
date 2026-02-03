using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Azure.Monitor.OpenTelemetry.Exporter;

namespace AzureApplicationInsightsLogger;

/// <summary>
/// Provides configuration and setup for Azure Application Insights logging with OpenTelemetry.
/// </summary>
public class ApplicationInsightsLoggerConfiguration
{
    /// <summary>
    /// Gets or sets the Azure Application Insights connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service name for resource identification.
    /// </summary>
    public string ServiceName { get; set; } = "DefaultService";

    /// <summary>
    /// Gets or sets the service version.
    /// </summary>
    public string ServiceVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets whether to include console exporter (for debugging).
    /// </summary>
    public bool IncludeConsoleExporter { get; set; } = false;
}

/// <summary>
/// Factory for creating and configuring Azure Application Insights loggers.
/// </summary>
public static class ApplicationInsightsLoggerFactory
{
    /// <summary>
    /// Creates a logger factory configured for Azure Application Insights.
    /// </summary>
    /// <param name="configuration">The Application Insights configuration.</param>
    /// <returns>A configured ILoggerFactory instance.</returns>
    public static ILoggerFactory CreateLoggerFactory(ApplicationInsightsLoggerConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.ConnectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(configuration.ConnectionString));
        }

        var resource = ResourceBuilder.CreateDefault()
            .AddService(configuration.ServiceName, serviceVersion: configuration.ServiceVersion);

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resource);

                if (configuration.IncludeConsoleExporter)
                {
                    options.AddConsoleExporter();
                }

                options.AddAzureMonitorLogExporter(azureOptions =>
                {
                    azureOptions.ConnectionString = configuration.ConnectionString;
                });
            });
        });

        return loggerFactory;
    }

    /// <summary>
    /// Creates a logger with the specified name.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="name">The logger name (typically the class name).</param>
    /// <returns>An ILogger instance.</returns>
    public static ILogger CreateLogger(ILoggerFactory loggerFactory, string name)
    {
        return loggerFactory.CreateLogger(name);
    }
}

/// <summary>
/// Provides custom event logging helpers for Azure Application Insights.
/// </summary>
public static class CustomEventLogger
{
    /// <summary>
    /// Logs a user login event.
    /// </summary>
    public static void LogUserLogin(ILogger logger, string userId, DateTime loginTime)
    {
        logger.LogInformation(
            "{microsoft.custom_event.name} UserId: {UserId}, LoginTime: {LoginTime}",
            "UserLoginEvent", userId, loginTime);
    }

    /// <summary>
    /// Logs a purchase event.
    /// </summary>
    public static void LogPurchase(ILogger logger, string productId, decimal amount, string currency)
    {
        logger.LogInformation(
            "{microsoft.custom_event.name} ProductId: {ProductId}, Amount: {Amount}, Currency: {Currency}",
            "PurchaseEvent", productId, amount, currency);
    }

    /// <summary>
    /// Logs a high-value purchase warning.
    /// </summary>
    public static void LogHighValuePurchase(ILogger logger, decimal amount, decimal threshold)
    {
        logger.LogWarning(
            "{microsoft.custom_event.name} Amount: {Amount}, Threshold: {Threshold}",
            "HighValuePurchase", amount, threshold);
    }

    /// <summary>
    /// Logs a custom user action.
    /// </summary>
    public static void LogCustomAction(ILogger logger, string actionType, string userId, bool success)
    {
        logger.LogInformation(
            "{microsoft.custom_event.name} ActionType: {ActionType}, UserId: {UserId}, Success: {Success}",
            "CustomUserAction", actionType, userId, success);
    }

    /// <summary>
    /// Logs a generic custom event.
    /// </summary>
    public static void LogCustomEvent(ILogger logger, string eventName, Dictionary<string, object?> properties)
    {
        var propertyFormat = string.Join(", ", properties.Keys.Select(k => $"{k}: {{{k}}}"));
        var values = new object?[] { eventName }.Concat(properties.Values).ToArray();

        logger.LogInformation(
            "{microsoft.custom_event.name} " + propertyFormat,
            values);
    }
}
