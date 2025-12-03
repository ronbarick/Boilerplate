using System.Collections.Generic;
using Project.Domain.Entities.Notifications;
using Project.Domain.Notifications;

namespace Project.Application.Notifications
{
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

            definitions.Add(new NotificationDefinition(
                "App.Notifications.SystemAlert",
                "System Alert",
                "Important system alert",
                NotificationSeverity.Warn,
                requiresSubscription: false
            ));
        }
    }
}
