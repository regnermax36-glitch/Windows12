# Windows 12 Installer

## Full BAT Installer

`Windows12_Installer.bat` uses **all files** from the self-contained `UI_Components` build and patches Windows.

### What it does
1. Finds the full build folder (EXE + all DLLs)
2. Copies everything to `C:\Program Files\Windows12`
3. Patches Windows:
   - Local account / OOBE bypass
   - Telemetry disabled
   - Dark mode + left-aligned taskbar
   - Widgets and search box adjustments
4. Creates a Desktop shortcut
5. Launches `UI_Components.exe`

### How to use
1. Download the `Windows12-Installer-EXE` artifact from ShellHost CI.
2. Extract it so `UI_Components.exe` and all DLLs are together.
3. Place `Windows12_Installer.bat` next to that folder (or inside it).
4. Right-click the BAT → **Run as administrator**.
