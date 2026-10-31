# Windows 12 UI Massive Expansion - Project Rebirth

This update massively extends the UI to deliver a true Windows 12 experience:

## New / Enhanced Components
- **Start Menu**: Full modern design with Search+Copilot, Pinned Apps, Recommended, User Profile, Power options, All Apps.
- **Floating Taskbar**: Centered floating dock with system tray (clock, notifications, system), Task View, Search, Widgets.
- **Settings App**: Complete navigation for System, Personalization, Time-Travel, Dev Workspace, Privacy (telemetry off by default).
- **File Explorer**: Modern simulation with sidebar (This PC, Dev Workspace, Snapshots), toolbar, grid view.
- **Notification Center**: Quick actions, sample notifications for Time-Travel and Dev Mode.
- **Themes**: Enhanced Sunset with Acrylic, new Dark Carbon theme, better shadows and corners (24px+).

## Features Added
- Local NPU search and Copilot hints (no cloud)
- Time-Travel integration points
- Dev Workspace mode indicators
- Floating, unanchored, modular UI elements
- Touch-optimized 40px+ targets
- High-refresh animation ready

## ISO Build Workflow
New `.github/workflows/build_iso.yml` packages the full source distribution into a Windows12_Rebirth.iso artifact using genisoimage.

Build on Windows for native binaries, or use the ISO as a complete source archive.

All designs follow the original Sunset / Carbon philosophy while expanding to production-like completeness.
