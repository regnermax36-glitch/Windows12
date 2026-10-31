# Windows 12 Installer

## BAT Launcher

`Windows12_Installer.bat` launches the real **UI_Components.exe** (the WinUI InstallerPage).

### How to use
1. Download the `Windows12-Installer-EXE` artifact from a successful ShellHost CI run.
2. Extract it so `UI_Components.exe` and all its DLLs are in one folder.
3. Place `Windows12_Installer.bat` in that same folder (or next to the extracted folder).
4. Double-click the BAT.

The BAT finds `UI_Components.exe`, sets the working directory so all dependent files load, and starts the installer UI.
