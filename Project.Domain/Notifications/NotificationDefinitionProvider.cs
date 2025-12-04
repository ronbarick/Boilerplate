using System.Collections.Generic;

namespace Project.Domain.Notifications
{
    public abstract class NotificationDefinitionProvider
    {
        public abstract void Define(List<NotificationDefinition> definitions);
    }
}
