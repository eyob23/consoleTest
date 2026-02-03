# Real-World Usage Examples

Complete, copy-paste ready examples for using AzureApplicationInsightsLogger in production scenarios.

## Example 1: E-Commerce Web API

### Program.cs
```csharp
using AzureApplicationInsightsLogger;
using MyECommerce.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add Azure Application Insights Logger
builder.Services.AddAzureApplicationInsightsLogging(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.ServiceName = "ECommerceAPI";
    options.ServiceVersion = builder.Configuration["Application:Version"] ?? "1.0.0";
    options.IncludeConsoleExporter = builder.Environment.IsDevelopment();
});

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```

### appsettings.json
```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=...;IngestionEndpoint=...;",
    "ServiceName": "ECommerceAPI"
  },
  "Application": {
    "Version": "2.1.0"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### OrderController.cs
```csharp
using AzureApplicationInsightsLogger;
using Microsoft.AspNetCore.Mvc;

namespace MyECommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrderService _orderService;

    public OrdersController(ILogger<OrdersController> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            // Log user action
            CustomEventLogger.LogCustomAction(
                _logger,
                actionType: "OrderInitiated",
                userId: User.Identity?.Name ?? "Anonymous",
                success: true
            );

            // Process order
            var order = await _orderService.CreateOrderAsync(request);

            // Log purchase event
            CustomEventLogger.LogPurchase(
                _logger,
                productId: request.ProductIds.FirstOrDefault() ?? "MULTI",
                amount: request.TotalAmount,
                currency: request.Currency
            );

            // Log warning for high-value purchases
            if (request.TotalAmount > 1000)
            {
                CustomEventLogger.LogHighValuePurchase(
                    _logger,
                    amount: request.TotalAmount,
                    threshold: 1000
                );
            }

            return Ok(new { orderId = order.Id, status = "Created" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for user {UserId}", User.Identity?.Name);
            return StatusCode(500, "Failed to create order");
        }
    }

    [HttpPost("{orderId}/checkout")]
    public async Task<IActionResult> Checkout(string orderId)
    {
        var checkoutProps = new Dictionary<string, object?>
        {
            { "OrderId", orderId },
            { "CheckoutTime", DateTime.UtcNow },
            { "PaymentMethod", "CreditCard" },
            { "UserId", User.Identity?.Name }
        };

        CustomEventLogger.LogCustomEvent(_logger, "CheckoutStarted", checkoutProps);
        
        // Process checkout...
        
        return Ok();
    }
}

public class CreateOrderRequest
{
    public List<string> ProductIds { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
}
```

---

## Example 2: Multi-Tenant SaaS Application

### TenantController.cs
```csharp
using AzureApplicationInsightsLogger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MySaaS.Controllers;

[ApiController]
[Route("api/tenants/{tenantId}/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ILogger<SubscriptionController> _logger;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(
        ILogger<SubscriptionController> logger,
        ISubscriptionService subscriptionService)
    {
        _logger = logger;
        _subscriptionService = subscriptionService;
    }

    [HttpPost("upgrade")]
    public async Task<IActionResult> UpgradeSubscription(
        string tenantId,
        [FromBody] UpgradeRequest request)
    {
        var userId = User.FindFirst("sub")?.Value ?? "Unknown";

        try
        {
            var result = await _subscriptionService.UpgradeAsync(tenantId, request);

            // Log subscription upgrade
            var upgradeProps = new Dictionary<string, object?>
            {
                { "TenantId", tenantId },
                { "UserId", userId },
                { "OldPlan", request.CurrentPlan },
                { "NewPlan", request.NewPlan },
                { "UpgradeDate", DateTime.UtcNow },
                { "AnnualCost", request.AnnualCost }
            };

            CustomEventLogger.LogCustomEvent(_logger, "SubscriptionUpgraded", upgradeProps);

            // Log purchase for financial tracking
            CustomEventLogger.LogPurchase(
                _logger,
                productId: request.NewPlan,
                amount: request.AnnualCost,
                currency: "USD"
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upgrade failed for tenant {TenantId}", tenantId);
            return StatusCode(500, "Upgrade failed");
        }
    }

    [HttpDelete("cancel")]
    public async Task<IActionResult> CancelSubscription(string tenantId)
    {
        var userId = User.FindFirst("sub")?.Value ?? "Unknown";

        CustomEventLogger.LogCustomAction(
            _logger,
            actionType: "SubscriptionCancellation",
            userId: userId,
            success: await _subscriptionService.CancelAsync(tenantId)
        );

        return Ok();
    }
}
```

---

## Example 3: Background Job Processing Service

### OrderProcessingService.cs
```csharp
using AzureApplicationInsightsLogger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyOrderProcessing.Services;

public class OrderProcessingService : BackgroundService
{
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly INotificationService _notificationService;

    public OrderProcessingService(
        ILogger<OrderProcessingService> logger,
        IOrderRepository orderRepository,
        IPaymentGateway paymentGateway,
        INotificationService notificationService)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _paymentGateway = paymentGateway;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pendingOrders = await _orderRepository.GetPendingOrdersAsync();
                
                _logger.LogInformation("Processing {OrderCount} pending orders", pendingOrders.Count);

                foreach (var order in pendingOrders)
                {
                    try
                    {
                        // Process payment
                        var paymentResult = await _paymentGateway.ProcessPaymentAsync(order);

                        if (paymentResult.Success)
                        {
                            // Log successful payment
                            CustomEventLogger.LogPurchase(
                                _logger,
                                productId: order.ProductId,
                                amount: order.Amount,
                                currency: order.Currency
                            );

                            // Update order status
                            order.Status = "Completed";
                            await _orderRepository.UpdateAsync(order);

                            // Notify customer
                            await _notificationService.SendOrderConfirmationAsync(order);

                            // Log action
                            CustomEventLogger.LogCustomAction(
                                _logger,
                                actionType: "OrderProcessed",
                                userId: order.CustomerId,
                                success: true
                            );
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Payment failed for order {OrderId}: {Reason}",
                                order.Id,
                                paymentResult.ErrorMessage);

                            order.Status = "PaymentFailed";
                            await _orderRepository.UpdateAsync(order);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception processing order {OrderId}", order.Id);
                    }
                }

                // Wait before next batch
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in order processing service");
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
```

---

## Example 4: User Authentication Service

### AuthenticationController.cs
```csharp
using AzureApplicationInsightsLogger;
using Microsoft.AspNetCore.Mvc;

namespace MyAuth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthenticationService _authService;

    public AuthController(
        ILogger<AuthController> logger,
        IAuthenticationService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.AuthenticateAsync(request.Email, request.Password);

            if (result.Success)
            {
                // Log successful login
                CustomEventLogger.LogUserLogin(
                    _logger,
                    userId: result.UserId,
                    loginTime: DateTime.UtcNow
                );

                // Log action
                CustomEventLogger.LogCustomAction(
                    _logger,
                    actionType: "LoginSuccess",
                    userId: result.UserId,
                    success: true
                );

                return Ok(new { token = result.Token });
            }
            else
            {
                // Log failed login attempt
                _logger.LogWarning(
                    "Failed login attempt for email {Email}",
                    request.Email);

                CustomEventLogger.LogCustomAction(
                    _logger,
                    actionType: "LoginFailed",
                    userId: request.Email,
                    success: false
                );

                return Unauthorized();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for {Email}", request.Email);
            return StatusCode(500, "Login failed");
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var userId = User.Identity?.Name ?? "Unknown";

        CustomEventLogger.LogCustomAction(
            _logger,
            actionType: "Logout",
            userId: userId,
            success: true
        );

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password);

            if (result.Success)
            {
                var userProps = new Dictionary<string, object?>
                {
                    { "UserId", result.UserId },
                    { "Email", request.Email },
                    { "RegisteredAt", DateTime.UtcNow },
                    { "Source", "WebAPI" }
                };

                CustomEventLogger.LogCustomEvent(
                    _logger,
                    "UserRegistered",
                    userProps
                );

                return Ok(new { userId = result.UserId });
            }

            return BadRequest("Registration failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error");
            return StatusCode(500, "Registration failed");
        }
    }
}
```

---

## Example 5: Custom Telemetry in Services

### AnalyticsService.cs
```csharp
using AzureApplicationInsightsLogger;
using Microsoft.Extensions.Logging;

namespace MyAnalytics.Services;

public interface IAnalyticsService
{
    Task<AnalyticsData> GetUserAnalyticsAsync(string userId);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly IAnalyticsRepository _repository;

    public AnalyticsService(
        ILogger<AnalyticsService> logger,
        IAnalyticsRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<AnalyticsData> GetUserAnalyticsAsync(string userId)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            var data = await _repository.GetAnalyticsAsync(userId);
            var duration = DateTime.UtcNow - startTime;

            // Log analytics query
            var queryProps = new Dictionary<string, object?>
            {
                { "UserId", userId },
                { "QueryDuration_ms", duration.TotalMilliseconds },
                { "RecordCount", data.Records?.Count ?? 0 },
                { "CacheHit", data.FromCache }
            };

            CustomEventLogger.LogCustomEvent(
                _logger,
                "AnalyticsQueryExecuted",
                queryProps
            );

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analytics query failed for user {UserId}", userId);
            throw;
        }
    }
}
```

---

## Azure Portal Query Examples

### Query 1: Track Purchase Revenue
```kusto
customEvents
| where name == "PurchaseEvent"
| extend Amount = todouble(customDimensions.Amount)
| summarize TotalRevenue=sum(Amount), TransactionCount=count() by tostring(customDimensions.Currency)
```

### Query 2: High-Value Transactions
```kusto
customEvents
| where name == "HighValuePurchase"
| extend Amount = todouble(customDimensions.Amount)
| order by Amount desc
| limit 50
```

### Query 3: Login Analytics
```kusto
customEvents
| where name == "UserLoginEvent"
| summarize LoginCount=count(), UniqueUsers=dcount(tostring(customDimensions.UserId)) by bin(timestamp, 1h)
```

### Query 4: User Action Funnel
```kusto
customEvents
| where name == "CustomUserAction"
| extend ActionType = tostring(customDimensions.ActionType)
| summarize ActionCount=count() by ActionType
| order by ActionCount desc
```

---

These examples demonstrate best practices for integrating AzureApplicationInsightsLogger into production applications!
