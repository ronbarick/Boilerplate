using ProjectRenamer.Models;
using System.Text.RegularExpressions;

namespace ProjectRenamer.Services;

/// <summary>
/// Specialized handler for updating .sln files.
/// </summary>
public class SolutionFileUpdater
{
    private readonly RenameOptions _options;

    public SolutionFileUpdater(RenameOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Updates the solution file with new project names and paths.
    /// </summary>
    public void UpdateSolutionFile()
    {
        Console.WriteLine("\n=== Updating Solution File ===");

        var solutionFiles = Directory.GetFiles(_options.SourcePath, "*.sln", SearchOption.TopDirectoryOnly);

        if (solutionFiles.Length == 0)
        {
            Console.WriteLine("No solution file found.");
            return;
        }

        foreach (var solutionFile in solutionFiles)
        {
            try
            {
                var content = File.ReadAllText(solutionFile);
                // Use regex to replace OldName but preserve "Project" keyword at start of line
                // Pattern matches OldName, but uses negative lookahead to ensure it's not followed by ("{
                // This prevents replacing Project("{GUID}") which is required by Visual Studio
                var pattern = "\\b" + Regex.Escape(_options.OldName) + "\\b(?!\\s*\\(\"\\{)";
                var newContent = System.Text.RegularExpressions.Regex.Replace(content, pattern, _options.NewName);

                if (content != newContent)
                {
                    if (_options.DryRun)
                    {
                        Console.WriteLine($"[DRY RUN] Would update: {solutionFile}");
                    }
                    else
                    {
                        File.WriteAllText(solutionFile, newContent);
                        Console.WriteLine($"Updated: {solutionFile}");
                    }
                }

                // Rename the solution file itself if needed
                var fileName = Path.GetFileName(solutionFile);
                if (fileName.Contains(_options.OldName))
                {
                    var newFileName = fileName.Replace(_options.OldName, _options.NewName);
                    var newPath = Path.Combine(Path.GetDirectoryName(solutionFile)!, newFileName);

                    if (_options.DryRun)
                    {
                        Console.WriteLine($"[DRY RUN] Would rename solution file: {solutionFile} -> {newPath}");
                    }
                    else
                    {
                        File.Move(solutionFile, newPath);
                        Console.WriteLine($"Renamed solution file: {solutionFile} -> {newPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating {solutionFile}: {ex.Message}");
            }
        }
    }
}
