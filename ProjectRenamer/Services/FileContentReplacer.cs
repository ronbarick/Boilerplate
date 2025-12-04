using ProjectRenamer.Models;
using System.Text.RegularExpressions;

namespace ProjectRenamer.Services;

/// <summary>
/// Handles replacing content in files.
/// </summary>
public class FileContentReplacer
{
    private readonly RenameOptions _options;
    private readonly string[] _fileExtensions = { ".cs", ".json", ".md", ".yml", ".yaml" };

    public FileContentReplacer(RenameOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Replaces all occurrences of the old project name with the new name in relevant files.
    /// Note: .csproj and .sln files are excluded and handled by their respective updaters.
    /// </summary>
    public void ReplaceInFiles()
    {
        Console.WriteLine("\n=== Replacing File Contents ===");

        var files = new List<string>();
        foreach (var extension in _fileExtensions)
        {
            files.AddRange(Directory.GetFiles(_options.SourcePath, $"*{extension}", SearchOption.AllDirectories));
        }

        // Exclude bin, obj, .git, .vs directories
        files = files.Where(f => !f.Contains("\\bin\\") && 
                                 !f.Contains("\\obj\\") && 
                                 !f.Contains("\\.git\\") && 
                                 !f.Contains("\\.vs\\"))
                     .ToList();

        int modifiedCount = 0;

        foreach (var file in files)
        {
            try
            {
                var content = File.ReadAllText(file);
                string newContent;

                // Simple replacement for all files (no longer handling .csproj here)
                newContent = content.Replace(_options.OldName, _options.NewName);

                // Special handling for launchSettings.json to preserve "commandName": "Project"
                if (Path.GetFileName(file).Equals("launchSettings.json", StringComparison.OrdinalIgnoreCase))
                {
                    // Revert the replacement for commandName if it was changed
                    // This fixes the issue where "commandName": "Project" becomes "commandName": "NewName"
                    // Visual Studio requires "commandName": "Project" to run the executable.
                    var invalidCommand = $"\"commandName\": \"{_options.NewName}\"";
                    var validCommand = "\"commandName\": \"Project\"";
                    newContent = newContent.Replace(invalidCommand, validCommand);
                }

                if (content != newContent)
                {
                    if (_options.DryRun)
                    {
                        Console.WriteLine($"[DRY RUN] Would modify: {file}");
                    }
                    else
                    {
                        File.WriteAllText(file, newContent);
                        Console.WriteLine($"Modified: {file}");
                    }
                    modifiedCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {file}: {ex.Message}");
            }
        }

        Console.WriteLine($"Modified {modifiedCount} files.");
    }
}
