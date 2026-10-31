@echo off
setlocal EnableDelayedExpansion
title Windows 12 Project Rebirth - Full System Installer
color 0B

echo.
echo ========================================================
echo          WINDOWS 12 PROJECT REBIRTH INSTALLER
echo     Deploys all DLLs / XAML / files and patches C:
echo ========================================================
echo.

net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] Must be run as Administrator.
    pause
    exit /b 1
)

:: Find the complete self-contained output
set "SOURCE_DIR="
if exist "%~dp0UI_Components.exe" set "SOURCE_DIR=%~dp0"
if not defined SOURCE_DIR if exist "%~dp0Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0Windows12-Installer-EXE\"
if not defined SOURCE_DIR if exist "%~dp0..\Windows12-Installer-EXE\UI_Components.exe" set "SOURCE_DIR=%~dp0..\Windows12-Installer-EXE\"
if not defined SOURCE_DIR if exist "%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\UI_Components.exe" (
    set "SOURCE_DIR=%~dp0..\src\UI_Components\bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\"
)

if not defined SOURCE_DIR (
    echo [ERROR] Build folder with UI_Components.exe not found.
    echo Extract the Windows12-Installer-EXE artifact next to this BAT.
    pause
    exit /b 1
)

echo Source folder: %SOURCE_DIR%
echo.

set "INSTALL_DIR=C:\Windows12"
set "SHELL_DIR=%INSTALL_DIR%\Shell"
set "OOBE_DIR=%INSTALL_DIR%\OOBE"
set "THEMES_DIR=%INSTALL_DIR%\Themes"

echo [1/8] Creating system directories on C:...
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
if not exist "%SHELL_DIR%" mkdir "%SHELL_DIR%"
if not exist "%OOBE_DIR%" mkdir "%OOBE_DIR%"
if not exist "%THEMES_DIR%" mkdir "%THEMES_DIR%"
echo        C:\Windows12 created.

echo.
echo [2/8] Copying ALL build files (EXE, DLLs, runtime, resources)...
xcopy "%SOURCE_DIR%*.*" "%SHELL_DIR%\" /E /I /Y /Q
echo        All files copied to %SHELL_DIR%

echo.
echo [3/8] Patching OOBE and local account on C:...
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE" /v BypassNRO /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\OOBE" /v DisablePrivacyExperience /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" /v ConsentPromptBehaviorAdmin /t REG_DWORD /d 0 /f >nul 2>&1
echo        OOBE patched.

echo.
echo [4/8] Disabling telemetry and error reporting...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v DoNotShowFeedbackNotifications /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting" /v Disabled /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy" /v TailoredExperiencesWithDiagnosticDataEnabled /t REG_DWORD /d 0 /f >nul 2>&1
echo        Telemetry disabled.

echo.
echo [5/8] Applying visual and shell patches...
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v AppsUseLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v SystemUsesLightTheme /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarAl /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarDa /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Search" /v SearchboxTaskbarMode /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v ShowTaskViewButton /t REG_DWORD /d 0 /f >nul 2>&1
echo        Visual patches applied.

echo.
echo [6/8] Writing installer marker and path...
reg add "HKLM\SOFTWARE\Windows12" /v InstallPath /t REG_SZ /d "%SHELL_DIR%" /f >nul 2>&1
reg add "HKLM\SOFTWARE\Windows12" /v Version /t REG_SZ /d "1.0" /f >nul 2>&1
echo        Registry path set.

echo.
echo [7/8] Creating Start Menu and Desktop shortcuts...
set "DESKTOP_LNK=%USERPROFILE%\Desktop\Windows 12 Shell.lnk"
set "START_LNK=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Windows 12 Shell.lnk"
powershell -NoProfile -Command "$s=(New-Object -ComObject WScript.Shell).CreateShortcut('%DESKTOP_LNK%'); $s.TargetPath='%SHELL_DIR%\UI_Components.exe'; $s.WorkingDirectory='%SHELL_DIR%'; $s.Save()" >nul 2>&1
powershell -NoProfile -Command "$s=(New-Object -ComObject WScript.Shell).CreateShortcut('%START_LNK%'); $s.TargetPath='%SHELL_DIR%\UI_Components.exe'; $s.WorkingDirectory='%SHELL_DIR%'; $s.Save()" >nul 2>&1
echo        Shortcuts created.

echo.
echo [8/8] Launching Windows 12 Shell / Installer...
if exist "%SHELL_DIR%\UI_Components.exe" (
    start "" "%SHELL_DIR%\UI_Components.exe"
    echo        UI_Components.exe started.
) else (
    echo        [WARN] UI_Components.exe not found after copy.
)

echo.
echo ========================================================
echo                    INSTALLATION COMPLETE
echo ========================================================
echo.
echo All files installed to: %SHELL_DIR%
echo System patches applied on C:.
echo Restart recommended for full effect.
echo.
pause
exit /b 0
