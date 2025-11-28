namespace Project.Core.Authorization;

/// <summary>
/// Represents a permission in the authorization system.
/// Used for defining permissions in code.
/// </summary>
public class Permission
{
    public string Name { get; }
    public string DisplayName { get; }
    public bool IsHostOnly { get; }
    public Permission? Parent { get; private set; }
    public List<Permission> Children { get; }

    public Permission(string name, string displayName, bool isHostOnly = false)
    {
        Name = name;
        DisplayName = displayName;
        IsHostOnly = isHostOnly;
        Children = new List<Permission>();
    }

    public Permission CreateChildPermission(string name, string displayName)
    {
        var child = new Permission(name, displayName, IsHostOnly)
        {
            Parent = this
        };
        Children.Add(child);
        return child;
    }
}
