using Project.Core.Entities.Notifications;

namespace Project.Core.Notifications
{
    public class NotificationDefinition
    {
        public string Name { get; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public NotificationSeverity Severity { get; set; }
        public bool RequiresSubscription { get; set; }

        public NotificationDefinition(string name, string displayName = null, string description = null, NotificationSeverity severity = NotificationSeverity.Info, bool requiresSubscription = true)
        {
            Name = name;
            DisplayName = displayName ?? name;
            Description = description;
            Severity = severity;
            RequiresSubscription = requiresSubscription;
        }
    }
}
