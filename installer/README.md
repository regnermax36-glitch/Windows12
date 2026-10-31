# Windows 12 Installer

## In-Place System Patcher

`Windows12_Installer.bat` patches the running Windows installation directly.

### What it does
1. Copies all build files (EXE + DLLs) into `%SystemRoot%\System32\Windows12`
2. Patches Explorer and taskbar behavior
3. Patches OOBE and telemetry
4. Registers the shell path
5. Adds the shell to startup
6. Restarts Explorer so changes take effect immediately

### How to use
1. Extract the `Windows12-Installer-EXE` artifact so `UI_Components.exe` and all DLLs are together.
2. Place `Windows12_Installer.bat` next to that folder.
3. Right-click the BAT → **Run as administrator**.
4. Restart if needed.

This modifies the current Windows installation instead of creating a separate program directory.
