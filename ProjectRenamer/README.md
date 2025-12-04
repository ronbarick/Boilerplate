# Project Renamer - Quick Start Guide

All the renamer tools are now in the **ProjectRenamer** folder!

## 📁 Folder Structure

```
Backend/
├── ProjectRenamer/              ← All renamer tools here
│   ├── create-project.bat       ← Create new project (recommended)
│   ├── rename-inplace.bat       ← Rename current boilerplate
│   ├── preview-rename.bat       ← Preview changes
│   ├── Program.cs               ← Console app
│   ├── ProjectRenamer.csproj    ← Project file
│   └── ...other files
├── Project.Application/
├── Project.Domain/
└── ...other project folders
```

---

## 🚀 How to Use (3 Easy Ways)

### Method 1: Command Line with Parameter ⭐ Recommended

```cmd
cd d:\Projects\Boilerplate\Boilerplate\Backend\ProjectRenamer
create-project.bat "MyNewProject"
```

### Method 2: Drag and Drop

1. Create a text file with your project name
2. Drag it onto `create-project.bat`

### Method 3: Double-Click (Interactive)

If you double-click the batch file without parameters, it will show usage instructions.

---

## 📝 Passing Project Name as Parameter

The batch files accept the project name as the **first parameter** in quotes:

### Example 1: Create New Project
```cmd
create-project.bat "MyAwesomeProject"
```

This creates a new project at: `d:\Projects\MyAwesomeProject`

### Example 2: Create with Custom Path
```cmd
create-project.bat "Acme.BookStore" "C:\Projects\AcmeBookStore"
```

### Example 3: Preview Changes
```cmd
preview-rename.bat "MyNewProject"
```

### Example 4: Rename In-Place
```cmd
rename-inplace.bat "BoilerplateProject"
```

---

## 🎯 Complete Step-by-Step Example

```cmd
REM Step 1: Navigate to ProjectRenamer folder
cd d:\Projects\Boilerplate\Boilerplate\Backend\ProjectRenamer

REM Step 2: (Optional) Preview what will change
preview-rename.bat "SchoolManagement"

REM Step 3: Create the new project
create-project.bat "SchoolManagement"

REM Done! Your new project is at d:\Projects\SchoolManagement
```

---

## 📋 Available Batch Files

### 1. `create-project.bat` - Create New Project

**Syntax:**
```cmd
create-project.bat "ProjectName" [DestinationPath]
```

**Examples:**
```cmd
REM Default destination (d:\Projects\ProjectName)
create-project.bat "MyProject"

REM Custom destination
create-project.bat "MyProject" "C:\Dev\MyProject"
```

**What it does:**
1. ✅ Copies Backend folder to destination
2. ✅ Renames all files and directories
3. ✅ Builds the project
4. ✅ Shows success message

---

### 2. `rename-inplace.bat` - Rename Current Boilerplate

**Syntax:**
```cmd
rename-inplace.bat "ProjectName"
```

**Example:**
```cmd
rename-inplace.bat "BoilerplateProject"
```

⚠️ **Warning:** This modifies the current boilerplate!

---

### 3. `preview-rename.bat` - Preview Changes

**Syntax:**
```cmd
preview-rename.bat "ProjectName" [Path]
```

**Examples:**
```cmd
REM Preview current directory
preview-rename.bat "MyProject"

REM Preview specific directory
preview-rename.bat "MyProject" "C:\Projects\MyProject"
```

---

## 🔧 Troubleshooting

### Error: "Project name is required!"

**Problem:** You didn't pass the project name as a parameter.

**Solution:** Add the project name in quotes:
```cmd
create-project.bat "MyProjectName"
```

### Error: "dotnet: command not found"

**Problem:** .NET SDK is not installed or not in PATH.

**Solution:** 
1. Install .NET 9 SDK from https://dotnet.microsoft.com/download
2. Restart your command prompt

### Error: "Access denied"

**Problem:** Files are locked by Visual Studio or another program.

**Solution:**
1. Close Visual Studio
2. Close any file explorers showing the Backend folder
3. Try again

---

## 💡 Tips

1. **Always use quotes** around the project name:
   ```cmd
   ✅ create-project.bat "MyProject"
   ❌ create-project.bat MyProject
   ```

2. **Project names with dots** (for namespaces):
   ```cmd
   create-project.bat "Acme.BookStore"
   ```

3. **Preview first** before creating:
   ```cmd
   preview-rename.bat "MyProject"
   create-project.bat "MyProject"
   ```

4. **Keep the original** boilerplate by using `create-project.bat` instead of `rename-inplace.bat`

---

## 📂 Where Files Are Created

By default, `create-project.bat` creates projects at:
```
d:\Projects\[YourProjectName]\
```

To use a different location, specify the second parameter:
```cmd
create-project.bat "MyProject" "C:\MyCustomPath"
```

---

## 🎓 Quick Reference

| Task | Command |
|------|---------|
| Create new project | `create-project.bat "MyProject"` |
| Create at custom path | `create-project.bat "MyProject" "C:\Path"` |
| Preview changes | `preview-rename.bat "MyProject"` |
| Rename in-place | `rename-inplace.bat "MyProject"` |
| Show help | Just run the .bat file without parameters |

---

## ✅ What Gets Renamed

- **396 files**: C# files, JSON, Markdown, etc.
- **8 .csproj files**: All project files
- **1 .sln file**: Solution file
- **11 directories**: All `Project.*` folders

From: `Project.*` → To: `YourProjectName.*`

---

Need more help? Check the detailed documentation in the ProjectRenamer folder!
