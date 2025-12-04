@echo off
REM ============================================================================
REM Project Renamer - Create New Project from Boilerplate
REM ============================================================================

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║          Project Renamer - Boilerplate Generator          ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

REM Check if project name was provided
if "%~1"=="" (
    echo ERROR: Project name is required!
    echo.
    echo Usage: create-project.bat "YourProjectName" [DestinationPath]
    echo.
    echo Examples:
    echo   create-project.bat "MyNewProject"
    echo   create-project.bat "Acme.BookStore" "C:\Projects\AcmeBookStore"
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

set PROJECT_NAME=%~1
set DESTINATION_PATH=%~2

REM If no destination path provided, use default
if "%DESTINATION_PATH%"=="" (
    set DESTINATION_PATH=d:\Projects\%PROJECT_NAME%
)

echo Project Name: %PROJECT_NAME%
echo Destination: %DESTINATION_PATH%
echo.

REM Check if destination already exists
if exist "%DESTINATION_PATH%" (
    echo WARNING: Destination directory already exists!
    echo %DESTINATION_PATH%
    echo.
    set /p OVERWRITE="Do you want to delete it and continue? (yes/no): "
    if /i not "%OVERWRITE%"=="yes" (
        echo Operation cancelled.
        pause
        exit /b 1
    )
    echo Deleting existing directory...
    rmdir /s /q "%DESTINATION_PATH%"
)

echo.
echo Step 1: Copying boilerplate to destination...
REM Get the parent directory (Backend folder)
set BACKEND_DIR=%~dp0..
xcopy /E /I /Q "%BACKEND_DIR%" "%DESTINATION_PATH%" /EXCLUDE:%~dp0exclude.txt
if errorlevel 1 (
    echo ERROR: Failed to copy files!
    pause
    exit /b 1
)
echo ✓ Copy completed successfully!

echo.
echo Step 2: Running renamer tool...
cd /d "%~dp0"
dotnet run --project . -- --name "%PROJECT_NAME%" --path "%DESTINATION_PATH%"
if errorlevel 1 (
    echo ERROR: Renamer failed!
    pause
    exit /b 1
)

echo.
echo Step 3: Building the renamed project...
cd /d "%DESTINATION_PATH%"
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
echo Your new project is ready at:
echo %DESTINATION_PATH%
echo.
echo Next steps:
echo 1. Open the solution in Visual Studio or VS Code
echo 2. Update the connection string in appsettings.json
echo 3. Run migrations: dotnet ef database update
echo.
pause
