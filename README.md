# Windows 12: Project "Rebirth" (Codename: Hudson Valley Next)

![Windows 12](https://cdn.neowin.com/news/images/uploaded/2024/01/1705485737_screenshot_1303.jpg)

## Executive Overview
Project "Rebirth" is a highly advanced, conceptual source code architecture simulating the impending 2027 release of Microsoft's Windows 12. Operating under the core philosophy of "Just Like Home," this operating system environment actively diverges from the industry trajectory of locked-down, cloud-reliant software. Instead, it restores absolute computing autonomy to the power user. 

Engineered entirely without mandatory telemetry or pre-installed bloatware, Project Rebirth focuses on deep-system modularity, native universal compatibility, and ultra-fluid, dynamic user interfaces built on WinUI 3 and C++23.

## Architectural Highlights

### 1. Universal Time-Travel Computing
Moving beyond standard shadow volume copies, the Time-Travel Engine runs as a background state-tracking daemon. It utilizes a highly efficient delta-storage layer, allowing developers and power users to flawlessly roll back the OS state down to the exact byte—eliminating the fear of system-breaking registry errors or corrupted driver installations.

### 2. Microsoft Dev Workspace OS Mode
A kernel-level toggle designed explicitly for elite developers. Engaging this mode instantly suppresses background consumer services, halts external telemetry pings, and reallocates maximum RAM capacity to integrated virtualization, localhost tools, and active code compilation.

### 3. Native Universal Package System (NUPS)
A registry-free, deep-system repository layout that seamlessly unifies Win32, UWP, WebApps, and native Linux binaries. NUPS handles dependency resolution cleanly, allowing for instantaneous installations and completely residue-free uninstalls.

### 4. Dynamic "Sunset" Interface & Retro-Shell
The default presentation features a vibrant, unanchored floating taskbar with a signature pink-and-yellow gradient. The user interface has been rigorously optimized for highly responsive, direct finger touch input on glass displays, bypassing any reliance on external styluses. For legacy enthusiasts, the "Retro-Shell" engine allows the system to seamlessly map classic UI skins (such as Windows XP's Luna or Windows 7's Aero) over the modern compositor. Included in the developer resources is also a Triple Black Carbon aesthetic for high-contrast coding environments.

## Massive UI Expansion (True Windows 12 Experience)
The presentation layer has been massively extended with production-like components:

- **Modern Start Menu**: Search + local Copilot, Pinned Apps, Recommended, user profile, power options
- **Floating Taskbar**: Centered dock with system tray (clock, notifications, system icons), Task View, Widgets
- **Full Settings App**: System, Personalization, Time-Travel controls, Dev Workspace, Privacy (telemetry disabled by default)
- **File Explorer Simulation**: Sidebar with This PC / Dev Workspace / Snapshots, modern toolbar and grid view
- **Notification Center**: Quick actions and system notifications
- **Enhanced Themes**: Acrylic Sunset, Dark Carbon, better shadows, larger corner radii

See `docs/UI_EXPANSION.md` for full details.

## Industry Context & Speculation
The architecture within this repository is heavily informed by—and in some cases, directly subverts—current industry intelligence regarding Microsoft's actual next-generation plans.

*   **The CorePC Foundation:** Current industry leaks covered by [PCWorld](https://www.pcworld.com/article/3068331/windows-12-rumors-features-pricing-everything-we-know-so-far.html) emphasize Microsoft's push toward a modular architecture. Project Rebirth adopts this CorePC structure to isolate components, but specifically strips out the rumored subscription walls.
*   **NPU Hardware Requirements:** Publications including [Tom's Guide](https://www.tomsguide.com/computing/windows-operating-systems/here-are-some-way-too-early-windows-12-predictions-based-on-how-microsoft-is-changing-windows-11) have highlighted the necessity of advanced Neural Processing Units (capable of 40+ TOPS) for next-gen Windows. We handle this natively in our `LocalAI_NPU` module, ensuring AI processes remain strictly on-device without cloud dependence.
*   **Official Microsoft Trajectory:** For continuing updates on official Windows announcements and developer tools, refer directly to the [Microsoft Windows Blog](https://blogs.windows.com/).

## Compilation & Hardware Requirements
Compiling the CorePC subsystem and the WinUI 3 ShellHost requires a formidable development environment. 

*   **Minimum Build Specifications:** Intel Core i5-13420H, dedicated GPU (RTX 4050 or equivalent), and 16GB of RAM.
*   **Target Compilation Environment:** 64GB of RAM, 4TB of high-speed NVMe storage, and a premium CPU (equivalent to Dell XPS 16 architecture) to successfully handle full kernel profiling and integrated virtualization without compilation bottlenecking.

## Building the Distribution ISO
A dedicated GitHub Actions workflow (`.github/workflows/build_iso.yml`) packages the complete source tree into a downloadable `Windows12_Rebirth.iso` artifact.

Trigger it manually via workflow_dispatch or on push to main. The ISO contains the full conceptual architecture ready for local builds on Windows with CMake + .NET 8 + Windows App SDK.

Native CI continues in `build_corepc.yml` and `shellhost_ci.yml`.
