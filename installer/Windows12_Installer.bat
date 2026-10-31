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

:: Check for admin rights
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] This installer requires Administrator privileges.
    echo Right-click the BAT file and select "Run as administrator".
    echo.
    pause
    exit /b 1
)

echo [1/6] Checking system...
ver | find "10." >nul
if errorlevel 1 (
    echo [WARN] This is intended for Windows 10/11. Continuing anyway...
)
echo        OK.

echo.
echo [2/6] Creating Windows 12 directories...
set "INSTALL_DIR=%ProgramFiles%\Windows12"
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
if not exist "%INSTALL_DIR%\Shell" mkdir "%INSTALL_DIR%\Shell"
if not exist "%INSTALL_DIR%\OOBE" mkdir "%INSTALL_DIR%\OOBE"
if not exist "%INSTALL_DIR%\Themes" mkdir "%INSTALL_DIR%\Themes"
echo        Created: %INSTALL_DIR%

echo.
echo [3/6] Configuring local account settings...
:: Disable forced Microsoft account requirement (Windows 11 style)
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE" /v BypassNRO /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\OOBE" /v DisablePrivacyExperience /t REG_DWORD /d 1 /f >nul 2>&1
echo        Local account bypass enabled.

echo.
echo [4/6] Disabling telemetry...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v DoNotShowFeedbackNotifications /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy" /v TailoredExperiencesWithDiagnosticDataEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo        Telemetry disabled.

echo.
echo [5/6] Applying Rectify12-style visual preferences...
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v AppsUseLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v SystemUsesLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarAl /t REG_DWORD /d 0 /f >nul 2>&1
echo        Dark mode + left-aligned taskbar applied.

echo.
echo [6/6] Finalizing...
if not exist "%INSTALL_DIR%\README.txt" (
    echo Windows 12 Project Rebirth > "%INSTALL_DIR%\README.txt"
    echo Installed via BAT installer. >> "%INSTALL_DIR%\README.txt"
    echo No Microsoft account required. >> "%INSTALL_DIR%\README.txt"
    echo Telemetry disabled. >> "%INSTALL_DIR%\README.txt"
)

echo.
echo ========================================================
echo                    INSTALLATION COMPLETE
echo ========================================================
echo.
echo Install location: %INSTALL_DIR%
echo.
echo Next steps:
echo   - Restart your PC to apply all registry changes.
echo   - The UI_Components.exe (if present) can be run from the artifact.
echo   - This BAT is a lightweight setup script for the Windows 12 experience.
echo.
echo Press any key to exit...
pause >nul
exit /b 0
