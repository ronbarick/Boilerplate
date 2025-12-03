using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Application.Notifications.Dtos;
using Project.Application.Services;
using Project.Domain.Interfaces;
using Project.Domain.Interfaces.Notifications;
using Project.Domain.Localization;

namespace Project.Application.Notifications
{
    public class NotificationAppService : AppServiceBase, INotificationAppService
    {
        private readonly IUserNotificationManager _userNotificationManager;

        public NotificationAppService(
            IUserNotificationManager userNotificationManager,
            ICurrentUser currentUser,
            ICurrentTenant currentTenant,
            IPermissionChecker permissionChecker,
            ILocalizationManager localizationManager)
            : base(currentUser, currentTenant, permissionChecker, localizationManager)
        {
            _userNotificationManager = userNotificationManager;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(GetNotificationsInput input)
        {
            var userId = CurrentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
            
            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
                userId,
                input.SkipCount,
                input.MaxResultCount,
                input.State);

            return notifications.Select(un => new NotificationDto
            {
                Id = un.Id,
                NotificationName = un.Notification.NotificationName,
                Data = un.Notification.Data,
                Severity = un.Notification.Severity,
                State = un.State,
                CreationTime = un.CreatedOn,
                ReadTime = un.ReadTime
            }).ToList();
        }

        public async Task MarkAsReadAsync(Guid userNotificationId)
        {
            var userId = CurrentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
            await _userNotificationManager.MarkAsReadAsync(userId, userNotificationId);
        }

        public async Task MarkAllAsReadAsync()
        {
            var userId = CurrentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
            await _userNotificationManager.MarkAllAsReadAsync(userId);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            var userId = CurrentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
            return await _userNotificationManager.GetUnreadCountAsync(userId);
        }

        public async Task SubscribeAsync(SubscribeToNotificationInput input)
        {
            var userId = CurrentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
            await _userNotificationManager.SubscribeAsync(
                userId,
                input.NotificationName,
                input.EntityTypeName,
                input.EntityId);
        }

        public async Task UnsubscribeAsync(string notificationName)
        {
            var userId = CurrentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
            await _userNotificationManager.UnsubscribeAsync(userId, notificationName);
        }
    }
}
