@echo off
REM ============================================================================
REM Project Renamer - Rename Current Boilerplate In-Place
REM ============================================================================

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║          Project Renamer - Rename In-Place                ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

REM Check if project name was provided
if "%~1"=="" (
    echo ERROR: Project name is required!
    echo.
    echo Usage: rename-inplace.bat "YourProjectName"
    echo.
    echo Example:
    echo   rename-inplace.bat "BoilerplateProject"
    echo.
    echo WARNING: This will rename the current boilerplate directory!
    echo          Make sure you have a backup or use create-project.bat instead.
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

set PROJECT_NAME=%~1
REM Get the parent directory (Backend folder)
set BACKEND_DIR=%~dp0..

echo Project Name: %PROJECT_NAME%
echo Current Path: %BACKEND_DIR%
echo.
echo WARNING: This will modify the current boilerplate!
echo.
set /p CONFIRM="Are you sure you want to continue? (yes/no): "
if /i not "%CONFIRM%"=="yes" (
    echo Operation cancelled.
    pause
    exit /b 1
)

echo.
echo Running renamer tool...
cd /d "%~dp0"
dotnet run --project . -- --name "%PROJECT_NAME%" --path "%BACKEND_DIR%"
if errorlevel 1 (
    echo ERROR: Renamer failed!
    pause
    exit /b 1
)

echo.
echo Building the renamed project...
cd /d "%BACKEND_DIR%"
dotnet build
if errorlevel 1 (
    echo WARNING: Build failed! Please check for errors.
    pause
    exit /b 1
)

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                  SUCCESS!                                  ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo The boilerplate has been renamed to: %PROJECT_NAME%
echo.
pause
