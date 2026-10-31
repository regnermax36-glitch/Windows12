# Rectify12 Massive Expansion

Added 80+ additional files to bring the project much closer to a full Windows 12 / Rectify11-style completeness:

## New Controls
AccentButton, Card, IconButton, SearchBox, AcrylicPanel, FloatingPanel, ClockWidget, WeatherWidget, TitleBar, and more.

## New Apps
Music, Calculator, Calendar, Mail, Store, Camera, Notes, Videos, Maps, Weather, News, Security, Update, Display, Sound, Network, Bluetooth, Accessibility, Power, and others.

All follow the Rectify12 theme resources and can be composed into the shell.

## Build Fix
Removed hardcoded RuntimeIdentifier from the csproj to resolve NETSDK1082 runtime pack errors. Platform is forced to x64. Self-contained build now relies on the CI command line RID.
