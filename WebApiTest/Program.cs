using AzureApplicationInsightsLogger;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogFileLogging(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddAzureApplicationInsightsLogging(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"] ?? string.Empty;
    options.ServiceName = builder.Configuration["ApplicationInsights:ServiceName"] ?? "WebApiTest";
    options.ServiceVersion = builder.Configuration["ApplicationInsights:ServiceVersion"] ?? "1.0.0";
    options.IncludeConsoleExporter = builder.Environment.IsDevelopment();
    options.UseAzureMonitor = builder.Configuration.GetValue<bool>("ApplicationInsights:UseAzureMonitor");
});

var app = builder.Build();

app.MapControllers();

app.Run();
