# Windows 12 Installer

## BAT Installer (recommended fallback)

`Windows12_Installer.bat` is a lightweight setup script that:

- Requires Administrator rights
- Creates `C:\Program Files\Windows12` directories
- Enables local account bypass (no Microsoft account forced)
- Disables telemetry
- Applies dark mode and left-aligned taskbar preferences

### How to use
1. Download `Windows12_Installer.bat`
2. Right-click → **Run as administrator**
3. Follow the prompts
4. Restart when finished

## UI_Components.exe
The self-contained WinUI installer EXE is produced by the ShellHost CI as the `Windows12-Installer-EXE` artifact.
If the EXE does not start, use the BAT instead.
