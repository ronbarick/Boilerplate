namespace ProjectRenamer.Models;

/// <summary>
/// Configuration options for the project renaming operation.
/// </summary>
public class RenameOptions
{
    /// <summary>
    /// The root directory path containing the project to rename.
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// The old project name to replace (default: "Project").
    /// </summary>
    public string OldName { get; set; } = "Project";

    /// <summary>
    /// The new project name.
    /// </summary>
    public string NewName { get; set; } = string.Empty;

    /// <summary>
    /// If true, performs a dry run without making actual changes.
    /// </summary>
    public bool DryRun { get; set; }

    /// <summary>
    /// Validates the options.
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(SourcePath))
        {
            errorMessage = "Source path is required.";
            return false;
        }

        if (!Directory.Exists(SourcePath))
        {
            errorMessage = $"Source path does not exist: {SourcePath}";
            return false;
        }

        if (string.IsNullOrWhiteSpace(NewName))
        {
            errorMessage = "New project name is required.";
            return false;
        }

        if (!IsValidIdentifier(NewName))
        {
            errorMessage = $"'{NewName}' is not a valid C# identifier. Use only letters, digits, and underscores, and start with a letter.";
            return false;
        }

        if (OldName.Equals(NewName, StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "Old name and new name cannot be the same.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private static bool IsValidIdentifier(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Must start with letter or underscore
        if (!char.IsLetter(name[0]) && name[0] != '_')
            return false;

        // Rest must be letters, digits, or underscores
        return name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
    }
}
