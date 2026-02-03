using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AzureApplicationInsightsLogger;

/// <summary>
/// Extension methods for configuring Serilog-based file logging.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Adds Serilog file logging using the provided configuration (Serilog section in appsettings).
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The host builder for chaining.</returns>
    public static IHostBuilder AddSerilogFileLogging(
        this IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        return hostBuilder.UseSerilog((_, loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext());
    }
}