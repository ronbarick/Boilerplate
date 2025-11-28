# Notification System - Architecture & Usage Guide

## Overview

This is a full-featured notification system built following ABP Framework patterns. It provides persistent notifications, real-time delivery via SignalR, user/tenant-specific notifications, and extensible notification definitions.

## Architecture

### Core Components

#### 1. **Entities** (`Project.Core.Entities.Notifications`)

- **Notification**: Stores the actual notification data
  - `Id`: Unique identifier
  - `NotificationName`: Type of notification (e.g., "App.Notifications.NewOrderCreated")
  - `Data`: JSON payload
  - `Severity`: Info, Success, Warn, Error, Fatal
  - `CreationTime`: When the notification was created

- **UserNotification**: Links notifications to users and tracks read state
  - `Id`: Unique identifier
  - `UserId`: Target user
  - `TenantId`: Tenant scope (nullable for host)
  - `NotificationId`: Foreign key to Notification
  - `State`: Unread / Read
  - `CreationTime`: When assigned to user
  - `ReadTime`: When marked as read (nullable)

- **NotificationSubscription**: Stores user subscriptions
  - `Id`: Unique identifier
  - `UserId`: Subscribed user
  - `TenantId`: Tenant scope (nullable)
  - `NotificationName`: Notification type
  - `EntityTypeName`: Optional entity filter
  - `EntityId`: Optional entity instance filter
  - `CreationTime`: When subscription was created

#### 2. **Interfaces** (`Project.Core.Interfaces.Notifications`)

- **INotificationPublisher**: Publishes notifications
  ```csharp
  Task PublishAsync(string notificationName, object data, Guid[] userIds, NotificationSeverity severity, Guid? tenantId);
  Task PublishToAllAsync(string notificationName, object data, NotificationSeverity severity);
  Task PublishToTenantAsync(Guid tenantId, string notificationName, object data, NotificationSeverity severity);
  ```

- **INotificationStore**: Persists and retrieves notifications
  ```csharp
  Task InsertNotificationAsync(Notification notification);
  Task InsertUserNotificationAsync(UserNotification userNotification);
  Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId, int skipCount, int maxResultCount, NotificationState? state);
  Task UpdateUserNotificationStateAsync(Guid userId, Guid userNotificationId, NotificationState state);
  Task<int> GetUnreadNotificationCountAsync(Guid userId);
  Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, string entityTypeName, string entityId);
  Task InsertSubscriptionAsync(NotificationSubscription subscription);
  Task DeleteSubscriptionAsync(Guid userId, string notificationName, string entityTypeName, string entityId);
  ```

- **IRealTimeNotifier**: Sends real-time notifications via SignalR
  ```csharp
  Task SendNotificationsAsync(UserNotification[] userNotifications);
  ```

- **INotificationDefinitionManager**: Manages notification type definitions
  ```csharp
  void Add(NotificationDefinition definition);
  NotificationDefinition Get(string name);
  IReadOnlyList<NotificationDefinition> GetAll();
  ```

- **IUserNotificationManager**: User-facing notification operations
  ```csharp
  Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId, int skipCount, int maxResultCount, NotificationState? state);
  Task MarkAsReadAsync(Guid userId, Guid userNotificationId);
  Task MarkAllAsReadAsync(Guid userId);
  Task<int> GetUnreadCountAsync(Guid userId);
  Task SubscribeAsync(Guid userId, string notificationName, string entityTypeName, string entityId);
  Task UnsubscribeAsync(Guid userId, string notificationName, string entityTypeName, string entityId);
  ```

#### 3. **Notification Definitions** (`Project.Core.Notifications`)

Define notification types with metadata:

```csharp
public class AppNotificationDefinitionProvider : NotificationDefinitionProvider
{
    public override void Define(List<NotificationDefinition> definitions)
    {
        definitions.Add(new NotificationDefinition(
            "App.Notifications.NewUserRegistered",
            "New User Registered",
            "A new user has registered in the system",
            NotificationSeverity.Info,
            requiresSubscription: false
        ));

        definitions.Add(new NotificationDefinition(
            "App.Notifications.NewOrderCreated",
            "New Order Created",
            "A new order has been created",
            NotificationSeverity.Success,
            requiresSubscription: true
        ));
    }
}
```

### Database Schema

#### Tables

1. **Notifications**
   - Primary Key: `Id` (GUID)
   - Indexes: `CreationTime`
   - Constraints: `NotificationName` (max 128 chars), `Data` (max 1MB)

2. **UserNotifications**
   - Primary Key: `Id` (GUID)
   - Foreign Key: `NotificationId` → `Notifications.Id` (CASCADE DELETE)
   - Indexes: `(UserId, State)`, `CreationTime`

3. **NotificationSubscriptions**
   - Primary Key: `Id` (GUID)
   - Indexes: `(UserId, NotificationName, EntityTypeName, EntityId)`, `NotificationName`

## Usage Examples

### 1. Publishing Notifications

#### To Specific Users
```csharp
public class OrderAppService
{
    private readonly INotificationPublisher _notificationPublisher;

    public async Task CreateOrderAsync(CreateOrderDto input)
    {
        // ... create order logic ...

        // Notify specific users
        var userIds = new[] { order.CreatedBy, order.AssignedTo };
        await _notificationPublisher.PublishAsync(
            "App.Notifications.NewOrderCreated",
            new { OrderId = order.Id, OrderNumber = order.Number },
            userIds,
            NotificationSeverity.Success,
            CurrentTenant.Id
        );
    }
}
```

#### To All Subscribed Users
```csharp
// Users must subscribe first
await _notificationPublisher.PublishAsync(
    "App.Notifications.NewOrderCreated",
    new { OrderId = order.Id },
    null, // null = use subscriptions
    NotificationSeverity.Info
);
```

#### To All Users in Tenant
```csharp
await _notificationPublisher.PublishToTenantAsync(
    tenantId,
    "App.Notifications.SystemAlert",
    new { Message = "System maintenance scheduled" },
    NotificationSeverity.Warn
);
```

#### To All Users (Host + All Tenants)
```csharp
await _notificationPublisher.PublishToAllAsync(
    "App.Notifications.SystemAlert",
    new { Message = "Critical system update" },
    NotificationSeverity.Error
);
```

### 2. Subscribing to Notifications

```csharp
public class NotificationAppService : INotificationAppService
{
    public async Task SubscribeToOrderNotificationsAsync()
    {
        await _userNotificationManager.SubscribeAsync(
            CurrentUser.Id.Value,
            "App.Notifications.NewOrderCreated"
        );
    }

    public async Task SubscribeToSpecificEntityAsync(Guid entityId)
    {
        await _userNotificationManager.SubscribeAsync(
            CurrentUser.Id.Value,
            "App.Notifications.EntityUpdated",
            "Order",
            entityId.ToString()
        );
    }
}
```

### 3. Reading Notifications

```csharp
// Get user notifications (paginated)
var notifications = await _userNotificationManager.GetUserNotificationsAsync(
    userId,
    skipCount: 0,
    maxResultCount: 20,
    state: NotificationState.Unread
);

// Mark as read
await _userNotificationManager.MarkAsReadAsync(userId, notificationId);

// Mark all as read
await _userNotificationManager.MarkAllAsReadAsync(userId);

// Get unread count
var count = await _userNotificationManager.GetUnreadCountAsync(userId);
```

### 4. SignalR Client Integration

#### JavaScript/TypeScript Client
```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notification-hub", {
        accessTokenFactory: () => getAuthToken()
    })
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
    // notification = {
    //     Id: "guid",
    //     NotificationName: "App.Notifications.NewOrderCreated",
    //     Data: "{\"OrderId\":\"...\",\"OrderNumber\":\"...\"}",
    //     Severity: "Success",
    //     CreationTime: "2025-11-27T10:00:00Z",
    //     State: "Unread"
    // }
    
    // Update UI
    showToast(notification);
    updateNotificationBadge();
});

await connection.start();
```

## Multi-Tenancy Support

### Tenant-Specific Notifications
```csharp
// Automatically scoped to current tenant
await _notificationPublisher.PublishAsync(
    "App.Notifications.NewOrder",
    data,
    userIds,
    NotificationSeverity.Info,
    CurrentTenant.Id // Tenant scope
);
```

### Host Notifications
```csharp
// Send to host users only
await _notificationPublisher.PublishAsync(
    "App.Notifications.SystemAlert",
    data,
    hostUserIds,
    NotificationSeverity.Warn,
    null // null = host scope
);
```

### SignalR Groups
The NotificationHub automatically manages user groups:
- `User_{UserId}`: Individual user group
- `Tenant_{TenantId}`: All users in a tenant

## Extensibility

### 1. Email Fallback Notifier

```csharp
public class EmailFallbackNotifier : IRealTimeNotifier
{
    private readonly IEmailSender _emailSender;
    private readonly IHubContext<NotificationHub> _hubContext;

    public async Task SendNotificationsAsync(UserNotification[] userNotifications)
    {
        foreach (var un in userNotifications)
        {
            // Try SignalR first
            await _hubContext.Clients.Group($"User_{un.UserId}")
                .SendAsync("ReceiveNotification", un);

            // Check if user is online, if not send email
            // (implement online user tracking)
            if (!IsUserOnline(un.UserId))
            {
                await _emailSender.SendAsync(
                    GetUserEmail(un.UserId),
                    "New Notification",
                    $"You have a new notification: {un.Notification.NotificationName}"
                );
            }
        }
    }
}
```

### 2. SMS Notifier

```csharp
public class SmsNotifier : IRealTimeNotifier
{
    private readonly ISmsService _smsService;

    public async Task SendNotificationsAsync(UserNotification[] userNotifications)
    {
        foreach (var un in userNotifications)
        {
            if (un.Notification.Severity >= NotificationSeverity.Error)
            {
                await _smsService.SendAsync(
                    GetUserPhoneNumber(un.UserId),
                    $"URGENT: {un.Notification.NotificationName}"
                );
            }
        }
    }
}
```

### 3. Distributed Notifier (Redis)

```csharp
public class RedisDistributedNotifier : IRealTimeNotifier
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHubContext<NotificationHub> _hubContext;

    public async Task SendNotificationsAsync(UserNotification[] userNotifications)
    {
        var subscriber = _redis.GetSubscriber();
        
        foreach (var un in userNotifications)
        {
            // Publish to Redis for multi-server scenarios
            await subscriber.PublishAsync(
                $"notifications:user:{un.UserId}",
                JsonSerializer.Serialize(un)
            );
        }
    }
}
```

### 4. Custom Receiver Filters

```csharp
public class RoleBasedNotificationPublisher : INotificationPublisher
{
    private readonly INotificationPublisher _basePublisher;
    private readonly IPermissionChecker _permissionChecker;

    public async Task PublishAsync(string notificationName, object data, Guid[] userIds, ...)
    {
        // Filter users by permission
        var authorizedUsers = new List<Guid>();
        foreach (var userId in userIds)
        {
            if (await _permissionChecker.IsGrantedAsync(userId, "Notifications.Receive"))
            {
                authorizedUsers.Add(userId);
            }
        }

        await _basePublisher.PublishAsync(
            notificationName,
            data,
            authorizedUsers.ToArray(),
            severity,
            tenantId
        );
    }
}
```

## Background Job Compatibility

The notification system is fully compatible with background jobs:

```csharp
public class DailyReportJob
{
    private readonly INotificationPublisher _notificationPublisher;

    public async Task ExecuteAsync()
    {
        // Generate report...
        
        // Notify users
        await _notificationPublisher.PublishToAllAsync(
            "App.Notifications.DailyReportReady",
            new { ReportDate = DateTime.UtcNow.Date },
            NotificationSeverity.Info
        );
    }
}
```

## API Endpoints

The `NotificationAppService` automatically exposes the following endpoints:

- `GET /api/app/notification/user-notifications` - Get user notifications
- `POST /api/app/notification/mark-as-read/{id}` - Mark notification as read
- `POST /api/app/notification/mark-all-as-read` - Mark all as read
- `GET /api/app/notification/unread-count` - Get unread count
- `POST /api/app/notification/subscribe` - Subscribe to notification
- `DELETE /api/app/notification/unsubscribe/{notificationName}` - Unsubscribe

## Caching

### Notification Definitions
Notification definitions are cached in memory via `INotificationDefinitionManager` (Singleton).

### Unread Counts (Optional Enhancement)
```csharp
public class CachedUserNotificationManager : IUserNotificationManager
{
    private readonly IDistributedCache _cache;
    private readonly IUserNotificationManager _baseManager;

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        var cacheKey = $"notifications:unread:{userId}";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (cached != null)
            return int.Parse(cached);

        var count = await _baseManager.GetUnreadCountAsync(userId);
        await _cache.SetStringAsync(cacheKey, count.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return count;
    }

    public async Task MarkAsReadAsync(Guid userId, Guid userNotificationId)
    {
        await _baseManager.MarkAsReadAsync(userId, userNotificationId);
        
        // Invalidate cache
        await _cache.RemoveAsync($"notifications:unread:{userId}");
    }
}
```

## Testing

### Unit Test Example
```csharp
[Fact]
public async Task Should_Publish_Notification_To_Subscribed_Users()
{
    // Arrange
    var userId = Guid.NewGuid();
    await _notificationStore.InsertSubscriptionAsync(new NotificationSubscription
    {
        UserId = userId,
        NotificationName = "Test.Notification"
    });

    // Act
    await _notificationPublisher.PublishAsync(
        "Test.Notification",
        new { Message = "Test" },
        null, // Use subscriptions
        NotificationSeverity.Info
    );

    // Assert
    var notifications = await _notificationStore.GetUserNotificationsAsync(userId, 0, 10);
    Assert.Single(notifications);
    Assert.Equal("Test.Notification", notifications[0].Notification.NotificationName);
}
```

## Migration & Database Setup

After implementing the notification system, run:

```bash
# Add migration
dotnet ef migrations add Added_Notifications --project Project.Migration --startup-project Project.WebApi

# Update database
dotnet ef database update --project Project.Migration --startup-project Project.WebApi
```

## Summary

This notification system provides:
- ✅ Persistent notifications with full audit trail
- ✅ Real-time delivery via SignalR
- ✅ User-specific and tenant-specific notifications
- ✅ Typed notification definitions
- ✅ Subscribe/unsubscribe functionality
- ✅ Read/unread tracking
- ✅ JSON-based flexible payload
- ✅ Background-job-safe delivery
- ✅ Scalable architecture with extensibility points
- ✅ Multi-tenancy support
- ✅ Auto-generated REST API endpoints

The system follows ABP Framework patterns and integrates seamlessly with the existing application architecture.
