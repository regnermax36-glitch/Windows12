# Windows 12 Installer

Modeled after the Rectify11 installer.

## What it does

- Installs to `C:\Windows\Windows12` (same pattern as Rectify11)
- Backs up key system files (`shell32.dll.mun`, `imageres.dll.mun`) before patching
- Patches the running Windows installation:
  - OOBE / local account bypass
  - Telemetry disabled
  - Dark mode, left taskbar, widgets off, Task View off
- Registers the shell path and adds it to startup
- Provides a Launch Shell button that shows the Windows 12 desktop UI

## Getting the EXE

1. Run the ShellHost CI workflow.
2. Download the `Windows12-Installer-EXE` artifact.
3. Run `UI_Components.exe` as Administrator.
4. Click Install, then Launch Shell.

## Console installer

A single-file console installer is also uploaded as `Windows12-Console-Installer`.
