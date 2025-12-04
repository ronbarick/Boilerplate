@echo off
REM ============================================================================
REM Project Renamer - Dry Run (Preview Changes)
REM ============================================================================

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║          Project Renamer - Dry Run Preview                ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

REM Check if project name was provided
if "%~1"=="" (
    echo ERROR: Project name is required!
    echo.
    echo Usage: preview-rename.bat "YourProjectName" [Path]
    echo.
    echo Examples:
    echo   preview-rename.bat "MyNewProject"
    echo   preview-rename.bat "Acme.BookStore" "C:\Projects\AcmeBookStore"
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

set PROJECT_NAME=%~1
set TARGET_PATH=%~2

REM If no path provided, use parent directory (Backend folder)
if "%TARGET_PATH%"=="" (
    set TARGET_PATH=%~dp0..
)

echo Project Name: %PROJECT_NAME%
echo Target Path: %TARGET_PATH%
echo.
echo This is a DRY RUN - no actual changes will be made.
echo.
pause

cd /d "%~dp0"
dotnet run --project . -- --name "%PROJECT_NAME%" --path "%TARGET_PATH%" --dry-run

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                  DRY RUN COMPLETE                          ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo No changes were made. This was just a preview.
echo.
echo To apply these changes, use:
echo   create-project.bat "%PROJECT_NAME%"
echo.
pause
