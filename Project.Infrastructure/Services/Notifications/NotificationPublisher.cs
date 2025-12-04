using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Domain.Entities.Notifications;
using Project.Domain.Interfaces.Notifications;
using System.Text.Json;

namespace Project.Infrastructure.Services.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly INotificationStore _store;
        private readonly IRealTimeNotifier _realTimeNotifier;
        private readonly INotificationDefinitionManager _definitionManager;

        public NotificationPublisher(
            INotificationStore store,
            IRealTimeNotifier realTimeNotifier,
            INotificationDefinitionManager definitionManager)
        {
            _store = store;
            _realTimeNotifier = realTimeNotifier;
            _definitionManager = definitionManager;
        }

        public async Task PublishAsync(string notificationName, object? data = null, Guid[]? userIds = null, NotificationSeverity severity = NotificationSeverity.Info, Guid? tenantId = null)
        {
            // 1. Create Notification
            var notification = new Notification
            {
                NotificationName = notificationName,
                Data = data != null ? JsonSerializer.Serialize(data) : null,
                Severity = severity
            };
            await _store.InsertNotificationAsync(notification);

            // 2. Determine Users
            var targetUserIds = new List<Guid>();
            if (userIds != null && userIds.Any())
            {
                targetUserIds.AddRange(userIds);
            }
            else
            {
                // Check subscriptions if no specific users provided
                var definition = _definitionManager.Get(notificationName);
                // If definition is null, we might assume it requires subscription or just don't send to anyone if no users specified.
                // Let's assume if definition exists and RequiresSubscription is true, we check.
                // If definition doesn't exist, maybe we shouldn't send? Or just proceed if users are specified.
                // Here users are NOT specified.
                
                if (definition != null && definition.RequiresSubscription)
                {
                    var subscriptions = await _store.GetSubscriptionsAsync(notificationName);
                    // Filter by tenant if needed
                    if (tenantId.HasValue)
                    {
                        subscriptions = subscriptions.Where(s => s.TenantId == tenantId || s.TenantId == null).ToList();
                    }
                    targetUserIds.AddRange(subscriptions.Select(s => s.UserId));
                }
            }

            // 3. Create UserNotifications
            var userNotifications = new List<UserNotification>();
            foreach (var userId in targetUserIds.Distinct())
            {
                var userNotification = new UserNotification
                {
                    UserId = userId,
                    TenantId = tenantId,
                    NotificationId = notification.Id,
                    State = NotificationState.Unread
                };
                await _store.InsertUserNotificationAsync(userNotification);
                userNotifications.Add(userNotification);
            }

            // 4. Send RealTime
            if (userNotifications.Any())
            {
                await _realTimeNotifier.SendNotificationsAsync(userNotifications.ToArray());
            }
        }

        public async Task PublishToAllAsync(string notificationName, object? data = null, NotificationSeverity severity = NotificationSeverity.Info)
        {
            await PublishAsync(notificationName, data, null, severity, null);
        }

        public async Task PublishToTenantAsync(Guid tenantId, string notificationName, object? data = null, NotificationSeverity severity = NotificationSeverity.Info)
        {
            await PublishAsync(notificationName, data, null, severity, tenantId);
        }
    }
}
