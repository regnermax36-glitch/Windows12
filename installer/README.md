# Windows 12 Installer

## Full System Installer

`Windows12_Installer.bat` deploys **all files** from the self-contained build and patches the Windows installation on C:.

### What it does
1. Locates the complete build folder (UI_Components.exe + every DLL and resource)
2. Copies everything to `C:\Windows12\Shell`
3. Patches the system:
   - OOBE / local account bypass
   - Telemetry and error reporting disabled
   - Dark mode, left taskbar, widgets off, Task View off
4. Writes install path to the registry
5. Creates Desktop and Start Menu shortcuts
6. Launches the shell/installer

### How to use
1. Download the `Windows12-Installer-EXE` artifact from a successful ShellHost CI run.
2. Extract it so all files (EXE + DLLs) are in one folder.
3. Place `Windows12_Installer.bat` next to that folder.
4. Right-click the BAT → **Run as administrator**.
5. Restart when finished.
