# Azure Application Insights Console App with OpenTelemetry

This is a simple console application that demonstrates how to send custom events to Azure Application Insights using OpenTelemetry.

## Prerequisites

- .NET 8.0 SDK or later
- An Azure Application Insights resource
- Connection string from your Application Insights instance

## Setup

1. **Get your Connection String**:
   - Go to Azure Portal → Application Insights → Overview
   - Copy the "Connection String"

2. **Set Environment Variable** (macOS/Linux):
   ```bash
   export APPLICATIONINSIGHTS_CONNECTION_STRING="your-connection-string-here"
   ```

   Or on Windows:
   ```cmd
   set APPLICATIONINSIGHTS_CONNECTION_STRING=your-connection-string-here
   ```

3. **Run the Application**:
   ```bash
   dotnet run
   ```

## What the App Does

The console app creates and logs custom events to Azure Application Insights:

1. **UserLoginEvent** - Logs a user login with userId and timestamp
2. **PurchaseEvent** - Logs a purchase event with product ID and amount
3. **HighValuePurchase** - Logs a warning for purchases over $50

Each event is logged with structured properties that will appear in Application Insights as custom events.

## Viewing Custom Events in Azure Portal

1. Go to Azure Application Insights
2. Navigate to **Logs** or **Custom Events** (Analytics)
3. Run a KQL query like:
   ```kusto
   customEvents
   | where name in ("UserLoginEvent", "PurchaseEvent", "HighValuePurchase")
   | project timestamp, name, tostring(customDimensions)
   ```

## Key Components

- **LoggerProvider**: Configures OpenTelemetry logging with Azure Monitor exporter
- **Resource**: Identifies the service (ConsoleApp v1.0.0)
- **BeginScope**: Groups custom properties with each log entry
- **CustomDimensions**: Properties sent to Application Insights as custom event dimensions

## Dependencies

- `OpenTelemetry`: Core OpenTelemetry library
- `Azure.Monitor.OpenTelemetry.Exporter`: Exports telemetry to Azure Monitor/Application Insights
- `OpenTelemetry.Exporter.Console`: Also outputs to console for local debugging
