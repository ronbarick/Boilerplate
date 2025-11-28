using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Core.Entities.Notifications;
using Project.Core.Interfaces.Notifications;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Services.Notifications
{
    public class NotificationStore : INotificationStore
    {
        private readonly AppDbContext _dbContext;

        public NotificationStore(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InsertNotificationAsync(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task InsertUserNotificationAsync(UserNotification userNotification)
        {
            await _dbContext.UserNotifications.AddAsync(userNotification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<UserNotification>> GetUserNotificationsAsync(Guid userId, int skipCount, int maxResultCount, NotificationState? state = null)
        {
            var query = _dbContext.UserNotifications
                .Include(un => un.Notification)
                .Where(un => un.UserId == userId);

            if (state.HasValue)
            {
                query = query.Where(un => un.State == state.Value);
            }

            return await query
                .OrderByDescending(un => un.CreatedOn)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }

        public async Task UpdateUserNotificationStateAsync(Guid userId, Guid userNotificationId, NotificationState state)
        {
            var userNotification = await _dbContext.UserNotifications
                .FirstOrDefaultAsync(un => un.UserId == userId && un.Id == userNotificationId);

            if (userNotification != null)
            {
                userNotification.State = state;
                if (state == NotificationState.Read)
                {
                    userNotification.ReadTime = DateTime.UtcNow;
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _dbContext.Notifications.Remove(notification);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
        {
            return await _dbContext.UserNotifications
                .CountAsync(un => un.UserId == userId && un.State == NotificationState.Unread);
        }

        public async Task<List<NotificationSubscription>> GetSubscriptionsAsync(string notificationName, string? entityTypeName = null, string? entityId = null)
        {
            var query = _dbContext.NotificationSubscriptions
                .Where(ns => ns.NotificationName == notificationName);

            if (entityTypeName != null)
            {
                query = query.Where(ns => ns.EntityTypeName == entityTypeName);
            }
            
            if (entityId != null)
            {
                query = query.Where(ns => ns.EntityId == entityId);
            }

            return await query.ToListAsync();
        }

        public async Task InsertSubscriptionAsync(NotificationSubscription subscription)
        {
            await _dbContext.NotificationSubscriptions.AddAsync(subscription);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteSubscriptionAsync(Guid userId, string notificationName, string? entityTypeName = null, string? entityId = null)
        {
            var query = _dbContext.NotificationSubscriptions
                .Where(ns => ns.UserId == userId && ns.NotificationName == notificationName);

            if (entityTypeName != null)
            {
                query = query.Where(ns => ns.EntityTypeName == entityTypeName);
            }

            if (entityId != null)
            {
                query = query.Where(ns => ns.EntityId == entityId);
            }

            var subscriptions = await query.ToListAsync();
            if (subscriptions.Any())
            {
                _dbContext.NotificationSubscriptions.RemoveRange(subscriptions);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
