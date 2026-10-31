@echo off
setlocal EnableDelayedExpansion
title Windows 12 Installer
color 0B

echo.
echo ========================================================
echo               WINDOWS 12 INSTALLER
echo ========================================================
echo.

:: Find the complete self-contained folder that contains UI_Components.exe
set "SOURCE_DIR="
if exist "%~dp0UI_Components.exe" set "SOURCE_DIR=%~dp0"
if not defined SOURCE_DIR if exist "%~dp0Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0Windows12-Installer-EXE\"
if not defined SOURCE_DIR if exist "%~dp0..\Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0..\Windows12-Installer-EXE\"
if not defined SOURCE_DIR if exist "%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\UI_Components.exe" (
    set "SOURCE_DIR=%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\"
)

if not defined SOURCE_DIR (
    echo [ERROR] UI_Components.exe not found.
    echo.
    echo Place this BAT in the same folder as UI_Components.exe
    echo or extract the Windows12-Installer-EXE artifact next to it.
    echo.
    pause
    exit /b 1
)

:: Remove trailing backslash if present for clean path handling
if "%SOURCE_DIR:~-1%"=="\" set "SOURCE_DIR=%SOURCE_DIR:~0,-1%"

echo Launching from: %SOURCE_DIR%
echo.

:: Change to the folder that contains every DLL and resource the EXE needs
cd /d "%SOURCE_DIR%"

:: Launch the exact same installer the CI builds
if exist "UI_Components.exe" (
    start "" /D "%SOURCE_DIR%" "UI_Components.exe"
    echo Installer started.
) else (
    echo [ERROR] UI_Components.exe missing after path resolution.
    pause
    exit /b 1
)

exit /b 0
