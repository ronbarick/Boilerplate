using ProjectRenamer.Models;
using System.Xml.Linq;

namespace ProjectRenamer.Services;

/// <summary>
/// Specialized handler for updating .csproj files.
/// </summary>
public class ProjectFileUpdater
{
    private readonly RenameOptions _options;

    public ProjectFileUpdater(RenameOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Updates all .csproj files with new project references and names.
    /// </summary>
    public void UpdateProjectFiles()
    {
        Console.WriteLine("\n=== Updating Project Files ===");

        var projectFiles = Directory.GetFiles(_options.SourcePath, "*.csproj", SearchOption.AllDirectories)
            .Where(f => !f.Contains("\\bin\\") && !f.Contains("\\obj\\"))
            .ToList();

        int updatedCount = 0;

        foreach (var projectFile in projectFiles)
        {
            try
            {
                var doc = XDocument.Load(projectFile);
                bool modified = false;

                // Update ProjectReference paths
                var projectReferences = doc.Descendants("ProjectReference");
                foreach (var reference in projectReferences)
                {
                    var includeAttr = reference.Attribute("Include");
                    if (includeAttr != null && includeAttr.Value.Contains(_options.OldName))
                    {
                        includeAttr.Value = includeAttr.Value.Replace(_options.OldName, _options.NewName);
                        modified = true;
                    }
                }

                // Update RootNamespace
                var rootNamespace = doc.Descendants("RootNamespace").FirstOrDefault();
                if (rootNamespace != null && rootNamespace.Value.Contains(_options.OldName))
                {
                    rootNamespace.Value = rootNamespace.Value.Replace(_options.OldName, _options.NewName);
                    modified = true;
                }

                // Update AssemblyName
                var assemblyName = doc.Descendants("AssemblyName").FirstOrDefault();
                if (assemblyName != null && assemblyName.Value.Contains(_options.OldName))
                {
                    assemblyName.Value = assemblyName.Value.Replace(_options.OldName, _options.NewName);
                    modified = true;
                }

                if (modified)
                {
                    if (_options.DryRun)
                    {
                        Console.WriteLine($"[DRY RUN] Would update: {projectFile}");
                    }
                    else
                    {
                        doc.Save(projectFile);
                        Console.WriteLine($"Updated: {projectFile}");
                    }
                    updatedCount++;
                }

                // Rename the project file itself if needed
                var fileName = Path.GetFileName(projectFile);
                if (fileName.Contains(_options.OldName))
                {
                    var newFileName = fileName.Replace(_options.OldName, _options.NewName);
                    var newPath = Path.Combine(Path.GetDirectoryName(projectFile)!, newFileName);

                    if (_options.DryRun)
                    {
                        Console.WriteLine($"[DRY RUN] Would rename project file: {projectFile} -> {newPath}");
                    }
                    else
                    {
                        // If we modified the content, we already saved it to 'projectFile'.
                        // Now we move it to the new name.
                        File.Move(projectFile, newPath);
                        Console.WriteLine($"Renamed project file: {projectFile} -> {newPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating {projectFile}: {ex.Message}");
            }
        }

        Console.WriteLine($"Updated {updatedCount} project files.");
    }
}
