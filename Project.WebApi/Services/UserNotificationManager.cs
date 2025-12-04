using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Domain.Entities.Notifications;
using Project.Domain.Interfaces.Notifications;

namespace Project.WebApi.Services
{
    public class UserNotificationManager : IUserNotificationManager
    {
        private readonly INotificationStore _store;

        public UserNotificationManager(INotificationStore store)
        {
            _store = store;
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId, int skipCount, int maxResultCount, NotificationState? state = null)
        {
            return await _store.GetUserNotificationsAsync(userId, skipCount, maxResultCount, state);
        }

        public async Task MarkAsReadAsync(Guid userId, Guid userNotificationId)
        {
            await _store.UpdateUserNotificationStateAsync(userId, userNotificationId, NotificationState.Read);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _store.GetUserNotificationsAsync(userId, 0, int.MaxValue, NotificationState.Unread);
            foreach (var notification in unreadNotifications)
            {
                await _store.UpdateUserNotificationStateAsync(userId, notification.Id, NotificationState.Read);
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _store.GetUnreadNotificationCountAsync(userId);
        }

        public async Task DeleteUserNotificationAsync(Guid userId, Guid userNotificationId)
        {
            var notification = await _store.GetUserNotificationsAsync(userId, 0, 1, null);
            if (notification.Count > 0 && notification[0].Id == userNotificationId)
            {
                await _store.DeleteNotificationAsync(notification[0].NotificationId);
            }
        }

        public async Task SubscribeAsync(Guid userId, string notificationName, string? entityTypeName = null, string? entityId = null)
        {
            var subscription = new NotificationSubscription
            {
                UserId = userId,
                NotificationName = notificationName,
                EntityTypeName = entityTypeName,
                EntityId = entityId
            };
            await _store.InsertSubscriptionAsync(subscription);
        }

        public async Task UnsubscribeAsync(Guid userId, string notificationName, string? entityTypeName = null, string? entityId = null)
        {
            await _store.DeleteSubscriptionAsync(userId, notificationName, entityTypeName, entityId);
        }
    }
}
