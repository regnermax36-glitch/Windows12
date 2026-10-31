@echo off
setlocal EnableDelayedExpansion
title Windows 12 Project Rebirth - Installer
color 0B

echo.
echo ========================================================
echo          WINDOWS 12 PROJECT REBIRTH INSTALLER
echo              No Bloat. No Telemetry. Full Control.
echo ========================================================
echo.

:: Locate the real UI_Components.exe produced by the ShellHost build
set "EXE="

:: 1. Same folder as this BAT
if exist "%~dp0UI_Components.exe" set "EXE=%~dp0UI_Components.exe"

:: 2. Parent folder
if not defined EXE if exist "%~dp0..\UI_Components.exe" set "EXE=%~dp0..\UI_Components.exe"

:: 3. Common artifact extract locations
if not defined EXE if exist "%~dp0Windows12-Installer-EXE\UI_Components.exe" set "EXE=%~dp0Windows12-Installer-EXE\UI_Components.exe"
if not defined EXE if exist "%~dp0..\Windows12-Installer-EXE\UI_Components.exe" set "EXE=%~dp0..\Windows12-Installer-EXE\UI_Components.exe"

:: 4. bin path if running from source tree
if not defined EXE if exist "%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\UI_Components.exe" (
    set "EXE=%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\UI_Components.exe"
)

if not defined EXE (
    echo [ERROR] Could not find UI_Components.exe
    echo.
    echo Place this BAT in the same folder as UI_Components.exe
    echo or extract the Windows12-Installer-EXE artifact next to it.
    echo.
    echo Expected locations:
    echo   - Same folder as this BAT
    echo   - Windows12-Installer-EXE\UI_Components.exe
    echo.
    pause
    exit /b 1
)

echo Found installer: %EXE%
echo.

:: Set working directory to the EXE folder so all dependent DLLs are found
for %%I in ("%EXE%") do set "EXE_DIR=%%~dpI"
cd /d "%EXE_DIR%"

echo Starting Windows 12 Installer UI...
echo.

:: Launch the real WinUI installer (InstallerPage)
start "" "%EXE%"

if %errorLevel% neq 0 (
    echo [ERROR] Failed to start UI_Components.exe
    echo Make sure all files from the artifact are in the same folder.
    pause
    exit /b 1
)

echo Installer launched. Close this window if the UI appears.
echo.
pause
exit /b 0
