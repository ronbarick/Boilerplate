using ProjectRenamer.Models;

namespace ProjectRenamer.Services;

/// <summary>
/// Handles renaming of project directories.
/// </summary>
public class DirectoryRenamer
{
    private readonly RenameOptions _options;

    public DirectoryRenamer(RenameOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Renames all directories matching the old project name pattern.
    /// </summary>
    public void RenameDirectories()
    {
        Console.WriteLine("\n=== Renaming Directories ===");

        var directories = Directory.GetDirectories(_options.SourcePath, $"{_options.OldName}.*", SearchOption.AllDirectories)
            .OrderByDescending(d => d.Length) // Process deepest directories first
            .ToList();

        // Also check root-level directories
        var rootDirs = Directory.GetDirectories(_options.SourcePath, $"{_options.OldName}.*", SearchOption.TopDirectoryOnly);
        foreach (var dir in rootDirs.Where(d => !directories.Contains(d)))
        {
            directories.Add(dir);
        }

        if (directories.Count == 0)
        {
            Console.WriteLine($"No directories found matching pattern '{_options.OldName}.*'");
            return;
        }

        foreach (var oldPath in directories)
        {
            var dirName = Path.GetFileName(oldPath);
            var newDirName = dirName.Replace(_options.OldName, _options.NewName);
            var newPath = Path.Combine(Path.GetDirectoryName(oldPath)!, newDirName);

            if (_options.DryRun)
            {
                Console.WriteLine($"[DRY RUN] Would rename: {oldPath} -> {newPath}");
            }
            else
            {
                Console.WriteLine($"Renaming: {oldPath} -> {newPath}");
                Directory.Move(oldPath, newPath);
            }
        }

        Console.WriteLine($"Renamed {directories.Count} directories.");
    }
}
