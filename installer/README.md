# Windows 12 Installer

Modeled after the Rectify11 installer approach.

## How it works

- Installs to `C:\Windows\Windows12` (same pattern as Rectify11's `C:\Windows\Rectify11`)
- Multi-step UI that shows progress
- Patches the running Windows installation in place:
  - OOBE / local account bypass
  - Telemetry disabled
  - Dark mode + left-aligned taskbar
- Requires Administrator rights
- Creates registry markers for the installation

## Getting the EXE

1. Run the ShellHost CI workflow (or wait for a push to main).
2. Download the `Windows12-Installer-EXE` artifact.
3. Extract it and run `UI_Components.exe` as Administrator.

## Console installer

A separate single-file console installer (`Windows12Installer.exe`) is also built and uploaded as the `Windows12-Console-Installer` artifact.
