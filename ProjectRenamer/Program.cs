using ProjectRenamer.Models;
using ProjectRenamer.Services;

namespace ProjectRenamer;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║          Project Renamer - Boilerplate Generator          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        var options = ParseArguments(args);

        if (options == null)
        {
            ShowUsage();
            return;
        }

        if (!options.IsValid(out string errorMessage))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {errorMessage}");
            Console.ResetColor();
            Console.WriteLine();
            ShowUsage();
            return;
        }

        // Display configuration
        Console.WriteLine("Configuration:");
        Console.WriteLine($"  Source Path: {options.SourcePath}");
        Console.WriteLine($"  Old Name:    {options.OldName}");
        Console.WriteLine($"  New Name:    {options.NewName}");
        Console.WriteLine($"  Dry Run:     {options.DryRun}");
        Console.WriteLine();

        // Confirm operation
        if (!options.DryRun)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WARNING: This operation will modify files and directories.");
            Console.WriteLine("Make sure you are working on a COPY of the boilerplate, not the original!");
            Console.ResetColor();
            Console.Write("\nDo you want to continue? (yes/no): ");
            var confirmation = Console.ReadLine()?.Trim().ToLower();

            if (confirmation != "yes" && confirmation != "y")
            {
                Console.WriteLine("Operation cancelled.");
                return;
            }
            Console.WriteLine();
        }

        try
        {
            // Execute renaming operations in order
            var fileReplacer = new FileContentReplacer(options);
            var projectUpdater = new ProjectFileUpdater(options);
            var solutionUpdater = new SolutionFileUpdater(options);
            var directoryRenamer = new DirectoryRenamer(options);

            // Step 1: Update file contents first (before renaming directories)
            fileReplacer.ReplaceInFiles();

            // Step 2: Update project files
            projectUpdater.UpdateProjectFiles();

            // Step 3: Update solution file
            solutionUpdater.UpdateSolutionFile();

            // Step 4: Rename directories last (so paths are still valid during file updates)
            directoryRenamer.RenameDirectories();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Project renaming completed successfully!");
            Console.ResetColor();

            if (options.DryRun)
            {
                Console.WriteLine("\nThis was a DRY RUN. No actual changes were made.");
                Console.WriteLine("Run without --dry-run to apply changes.");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n✗ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
        }
    }

    static RenameOptions? ParseArguments(string[] args)
    {
        var options = new RenameOptions();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--name":
                case "-n":
                    if (i + 1 < args.Length)
                        options.NewName = args[++i];
                    break;

                case "--path":
                case "-p":
                    if (i + 1 < args.Length)
                        options.SourcePath = args[++i];
                    break;

                case "--old-name":
                case "-o":
                    if (i + 1 < args.Length)
                        options.OldName = args[++i];
                    break;

                case "--dry-run":
                case "-d":
                    options.DryRun = true;
                    break;

                case "--help":
                case "-h":
                    return null;

                default:
                    Console.WriteLine($"Unknown argument: {args[i]}");
                    return null;
            }
        }

        // If no path specified, use current directory
        if (string.IsNullOrWhiteSpace(options.SourcePath))
        {
            options.SourcePath = Directory.GetCurrentDirectory();
        }

        return options;
    }

    static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  ProjectRenamer --name <NewProjectName> [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --name, -n <name>       New project name (required)");
        Console.WriteLine("  --path, -p <path>       Path to the project directory (default: current directory)");
        Console.WriteLine("  --old-name, -o <name>   Old project name to replace (default: 'Project')");
        Console.WriteLine("  --dry-run, -d           Preview changes without applying them");
        Console.WriteLine("  --help, -h              Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ProjectRenamer --name MyNewProject");
        Console.WriteLine("  ProjectRenamer --name Acme.BookStore --path C:\\Projects\\BoilerplateCopy");
        Console.WriteLine("  ProjectRenamer --name MyApp --dry-run");
        Console.WriteLine();
    }
}
