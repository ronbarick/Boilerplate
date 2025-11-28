using System.Collections.Generic;
using Project.Core.Notifications;

namespace Project.Core.Interfaces.Notifications
{
    public interface INotificationDefinitionManager
    {
        void Add(NotificationDefinition definition);
        NotificationDefinition Get(string name);
        IReadOnlyList<NotificationDefinition> GetAll();
    }
}
