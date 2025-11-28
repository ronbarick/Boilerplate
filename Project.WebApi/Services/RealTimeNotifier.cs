using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Project.Core.Entities.Notifications;
using Project.Core.Interfaces.Notifications;
using Project.WebApi.Hubs;

namespace Project.WebApi.Services
{
    public class RealTimeNotifier : IRealTimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealTimeNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                var notification = userNotification.Notification;
                
                var payload = new
                {
                    Id = userNotification.Id,
                    NotificationName = notification.NotificationName,
                    Data = notification.Data,
                    Severity = notification.Severity,
                    CreationTime = notification.CreatedOn,
                    State = userNotification.State
                };

                await _hubContext.Clients.Group($"User_{userNotification.UserId}")
                    .SendAsync("ReceiveNotification", payload);
            }
        }
    }
}
