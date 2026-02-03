using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureApplicationInsightsLogger;

/// <summary>
/// Extension methods for integrating Azure Application Insights logging into ASP.NET Core applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure Application Insights logging to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">An optional action to configure the Application Insights options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAzureApplicationInsightsLogging(
        this IServiceCollection services,
        Action<ApplicationInsightsLoggerConfiguration>? configureOptions = null)
    {
        var configuration = new ApplicationInsightsLoggerConfiguration();
        configureOptions?.Invoke(configuration);

        if (string.IsNullOrWhiteSpace(configuration.ConnectionString))
        {
            throw new InvalidOperationException(
                "Azure Application Insights connection string is required. " +
                "Please configure it using AddAzureApplicationInsightsLogging options.");
        }

        // Create and register the logger factory
        var loggerFactory = ApplicationInsightsLoggerFactory.CreateLoggerFactory(configuration);
        services.AddSingleton(loggerFactory);

        return services;
    }

    /// <summary>
    /// Adds Azure Application Insights logging with a connection string loaded from configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The Azure Application Insights connection string.</param>
    /// <param name="serviceName">Optional: The service name (defaults to "WebAPI").</param>
    /// <param name="serviceVersion">Optional: The service version (defaults to "1.0.0").</param>
    /// <param name="includeConsoleExporter">Optional: Whether to include console exporter for debugging.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAzureApplicationInsightsLogging(
        this IServiceCollection services,
        string connectionString,
        string serviceName = "WebAPI",
        string serviceVersion = "1.0.0",
        bool includeConsoleExporter = false)
    {
        return services.AddAzureApplicationInsightsLogging(options =>
        {
            options.ConnectionString = connectionString;
            options.ServiceName = serviceName;
            options.ServiceVersion = serviceVersion;
            options.IncludeConsoleExporter = includeConsoleExporter;
        });
    }
}
