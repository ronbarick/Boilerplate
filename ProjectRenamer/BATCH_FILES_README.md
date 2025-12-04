# Project Renamer - Batch Files

This directory contains batch files to simplify using the Project Renamer tool.

## Available Batch Files

### 1. `create-project.bat` - Create New Project (Recommended)

Creates a new project from the boilerplate by copying it to a new location and renaming.

**Usage:**
```cmd
create-project.bat "YourProjectName" [DestinationPath]
```

**Examples:**
```cmd
REM Create project in default location (d:\Projects\YourProjectName)
create-project.bat "MyNewProject"

REM Create project in custom location
create-project.bat "Acme.BookStore" "C:\Projects\AcmeBookStore"
```

**What it does:**
1. Copies the boilerplate to the destination
2. Runs the renamer tool
3. Builds the renamed project
4. Shows success message with next steps

---

### 2. `rename-inplace.bat` - Rename Current Boilerplate

Renames the current boilerplate directory in-place.

**Usage:**
```cmd
rename-inplace.bat "YourProjectName"
```

**Example:**
```cmd
rename-inplace.bat "BoilerplateProject"
```

> [!WARNING]
> This modifies the current boilerplate! Make sure you have a backup.

**What it does:**
1. Asks for confirmation
2. Runs the renamer tool on the current directory
3. Builds the renamed project

---

### 3. `preview-rename.bat` - Preview Changes (Dry Run)

Preview what changes would be made without actually applying them.

**Usage:**
```cmd
preview-rename.bat "YourProjectName" [Path]
```

**Examples:**
```cmd
REM Preview changes in current directory
preview-rename.bat "MyNewProject"

REM Preview changes in specific directory
preview-rename.bat "Acme.BookStore" "C:\Projects\AcmeBookStore"
```

**What it does:**
1. Shows all files and directories that would be modified
2. Does NOT make any actual changes
3. Useful for verifying before running the actual rename

---

## Quick Start

### For Most Users (Recommended)

```cmd
cd d:\Projects\Boilerplate\Boilerplate\Backend
create-project.bat "MyCompany.MyProduct"
```

This will:
- Create a copy at `d:\Projects\MyCompany.MyProduct`
- Rename everything from `Project` to `MyCompany.MyProduct`
- Build the project to verify it works

### Preview First, Then Create

```cmd
REM Step 1: Preview
preview-rename.bat "MyNewProject"

REM Step 2: If everything looks good, create
create-project.bat "MyNewProject"
```

---

## Project Name Rules

Your project name must be a valid C# identifier:
- ✅ Start with a letter or underscore
- ✅ Contain only letters, digits, underscores, or dots
- ✅ Examples: `MyProject`, `Acme.BookStore`, `School_Management`
- ❌ Cannot contain spaces or special characters

---

## What Gets Renamed

- **396 files**: `.cs`, `.sln`, `.json`, `.md`, `.yml`, `.yaml`
- **8 project files**: All `.csproj` files
- **1 solution file**: `Project.sln` → `YourName.sln`
- **11 directories**: All `Project.*` directories

---

## Troubleshooting

### "dotnet: command not found"
Make sure .NET SDK is installed and in your PATH.

### "Access denied" errors
Close Visual Studio and any other programs that might have files open.

### Build fails after renaming
1. Delete all `bin` and `obj` folders
2. Run `dotnet clean`
3. Run `dotnet build` again

---

## Manual Alternative

If you prefer to run commands manually:

```cmd
REM Copy the boilerplate
xcopy /E /I /Q d:\Projects\Boilerplate\Boilerplate\Backend d:\Projects\MyNewProject

REM Run the renamer
cd d:\Projects\Boilerplate\Boilerplate\Backend
dotnet run --project ProjectRenamer -- --name "MyNewProject" --path "d:\Projects\MyNewProject"

REM Build the renamed project
cd d:\Projects\MyNewProject
dotnet build
```
