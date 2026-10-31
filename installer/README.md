# Windows 12 Installer

## Exact Launcher

`Windows12_Installer.bat` launches **UI_Components.exe** exactly as the CI build produces it.

### How to use
1. Download the `Windows12-Installer-EXE` artifact from a successful ShellHost CI run.
2. Extract it so `UI_Components.exe` and all its DLLs are in one folder.
3. Place `Windows12_Installer.bat` in that same folder (or next to the extracted folder).
4. Double-click the BAT.

The BAT sets the working directory to the folder containing every required file and starts `UI_Components.exe`.
