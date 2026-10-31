@echo off
setlocal EnableDelayedExpansion
title Windows 12 Project Rebirth - System Patcher
color 0B

echo.
echo ========================================================
echo          WINDOWS 12 PROJECT REBIRTH PATCHER
echo     Applies changes directly to the running Windows
echo ========================================================
echo.

net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] Must be run as Administrator.
    pause
    exit /b 1
)

:: Locate the full build so we can use its files
set "SOURCE_DIR="
if exist "%~dp0UI_Components.exe" set "SOURCE_DIR=%~dp0"
if not defined SOURCE_DIR if exist "%~dp0Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0Windows12-Installer-EXE\"
if not defined SOURCE_DIR if exist "%~dp0..\Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0..\Windows12-Installer-EXE\"

if not defined SOURCE_DIR (
    echo [ERROR] UI_Components build folder not found next to this BAT.
    echo Extract the Windows12-Installer-EXE artifact here first.
    pause
    exit /b 1
)

echo Source: %SOURCE_DIR%
echo.

set "TARGET=%SystemRoot%\System32\Windows12"
if not exist "%TARGET%" mkdir "%TARGET%"

echo [1/6] Copying shell components into System32...
xcopy "%SOURCE_DIR%*.*" "%TARGET%\" /E /I /Y /Q >nul
echo        Files placed in %TARGET%

echo.
echo [2/6] Patching shell and explorer behavior...
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarAl /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarDa /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v ShowTaskViewButton /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Search" /v SearchboxTaskbarMode /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v AppsUseLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v SystemUsesLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
echo        Taskbar and theme patched.

echo.
echo [3/6] Patching OOBE and privacy...
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE" /v BypassNRO /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\OOBE" /v DisablePrivacyExperience /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting" /v Disabled /t REG_DWORD /d 1 /f >nul 2>&1
echo        OOBE and telemetry patched.

echo.
echo [4/6] Setting Windows 12 shell path in registry...
reg add "HKLM\SOFTWARE\Windows12" /v ShellPath /t REG_SZ /d "%TARGET%\UI_Components.exe" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Windows12" /v InstallPath /t REG_SZ /d "%TARGET%" /f >nul 2>&1
echo        Shell path registered.

echo.
echo [5/6] Adding to startup so shell loads...
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v Windows12Shell /t REG_SZ /d "%TARGET%\UI_Components.exe" /f >nul 2>&1
echo        Startup entry added.

echo.
echo [6/6] Restarting Explorer to apply patches...
taskkill /f /im explorer.exe >nul 2>&1
start explorer.exe
echo        Explorer restarted.

echo.
echo ========================================================
echo                    PATCH COMPLETE
echo ========================================================
echo.
echo Components are in: %TARGET%
echo System registry and explorer have been patched.
echo A restart is recommended for full effect.
echo.
pause
exit /b 0
