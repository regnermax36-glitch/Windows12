@echo off
setlocal EnableDelayedExpansion
title Windows 12 Project Rebirth - Full Installer
color 0B

echo.
echo ========================================================
echo          WINDOWS 12 PROJECT REBIRTH INSTALLER
echo         Uses all build files + patches Windows
echo ========================================================
echo.

:: Admin check
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] Run as Administrator.
    pause
    exit /b 1
)

:: Locate the full self-contained build folder
set "SOURCE_DIR="
if exist "%~dp0UI_Components.exe" set "SOURCE_DIR=%~dp0"
if not defined SOURCE_DIR if exist "%~dp0Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0Windows12-Installer-EXE\"
if not defined SOURCE_DIR if exist "%~dp0..\Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0..\Windows12-Installer-EXE\"
if not defined SOURCE_DIR if exist "%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\UI_Components.exe" (
    set "SOURCE_DIR=%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\"
)

if not defined SOURCE_DIR (
    echo [ERROR] Could not find the UI_Components build folder.
    echo Extract the Windows12-Installer-EXE artifact next to this BAT.
    pause
    exit /b 1
)

echo Source: %SOURCE_DIR%
echo.

set "INSTALL_DIR=%ProgramFiles%\Windows12"
echo [1/7] Creating install directory...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
echo        %INSTALL_DIR%

echo.
echo [2/7] Copying all files (EXE, DLLs, runtime)...
xcopy "%SOURCE_DIR%*.*" "%INSTALL_DIR%\" /E /I /Y /Q >nul
echo        Copied all build files.

echo.
echo [3/7] Patching OOBE / local account...
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE" /v BypassNRO /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\OOBE" /v DisablePrivacyExperience /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" /v EnableLUA /t REG_DWORD /d 1 /f >nul 2>&1
echo        Local account bypass applied.

echo.
echo [4/7] Disabling telemetry and feedback...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v DoNotShowFeedbackNotifications /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy" /v TailoredExperiencesWithDiagnosticDataEnabled /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting" /v Disabled /t REG_DWORD /d 1 /f >nul 2>&1
echo        Telemetry disabled.

echo.
echo [5/7] Applying Rectify12 visual patches...
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v AppsUseLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v SystemUsesLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarAl /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarDa /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Search" /v SearchboxTaskbarMode /t REG_DWORD /d 1 /f >nul 2>&1
echo        Dark mode, left taskbar, widgets off.

echo.
echo [6/7] Registering installer shortcut...
set "SHORTCUT=%USERPROFILE%\Desktop\Windows 12 Installer.lnk"
powershell -NoProfile -Command "$s=(New-Object -ComObject WScript.Shell).CreateShortcut('%SHORTCUT%'); $s.TargetPath='%INSTALL_DIR%\UI_Components.exe'; $s.WorkingDirectory='%INSTALL_DIR%'; $s.Save()" >nul 2>&1
echo        Desktop shortcut created.

echo.
echo [7/7] Launching Windows 12 Installer UI...
start "" "%INSTALL_DIR%\UI_Components.exe"
echo.

echo ========================================================
echo                    INSTALLATION COMPLETE
echo ========================================================
echo.
echo All files installed to: %INSTALL_DIR%
echo Windows has been patched (OOBE, telemetry, visuals).
echo Restart recommended.
echo.
pause
exit /b 0
