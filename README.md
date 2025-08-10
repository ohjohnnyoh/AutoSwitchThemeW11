# Auto Switch Theme W11

[![Version](https://img.shields.io/badge/version-1.5.1-blue.svg)](https://github.com/oh_johnny/AutoSwitchThemeW11/releases)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-green.svg)](https://www.microsoft.com/windows)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-yellow.svg)](LICENSE)

A lightweight Windows application that automatically switches system themes (Light/Dark) based on your schedule or manual commands from the system tray. Works seamlessly in the background with minimal system impact.

## 🛡️ Critical Safety Fixes v1.5.1

- **FIXED EXPLORER.EXE CRASHES** - completely removed aggressive window manipulation methods
- **ELIMINATED SCREEN FLICKERING** - safe synchronization without direct UI interference
- **SECURE ARCHITECTURE** - only basic system notifications
- **INCREASED TIMEOUTS** - 500ms instead of 50-100ms for maximum stability
- **ADDED PAUSES** - 100-200ms between notifications to prevent crashes
- **STABILITY OVER SPEED** - security priority over performance

## ✨ Key Features

- **Tray-only operation** - runs in system tray without main window
- **Instant theme switching** - Light/Dark themes with one click
- **Flexible scheduling** - set "Light from..." and "Dark from..." times, supports midnight transitions
- **Auto-start with Windows** - automatic startup management
- **Multi-language support** - Russian, English, French, German, Spanish
- **Minimal resource usage** - lightweight background operation
- **Comprehensive logging** - diagnostic logs for troubleshooting
- **Monitor diagnostics** - detailed multi-monitor configuration analysis
- **Automatic monitor detection** - detects all monitors and taskbar windows
- **Guaranteed synchronization** - syncs across all monitors
- **Optimized architecture** - maximum performance with safety

## 🖥️ System Requirements

- Windows 10/11 x64
- Self-contained .NET 8 build included - no separate .NET installation required
- Support for any monitor configuration (1-10+ monitors)
- Minimal system requirements thanks to optimization
- Full Explorer.exe compatibility without crashes

## 🚀 Installation & Usage

### Quick Start
1. Download and run `installer/Output/AutoSwitchThemeW11-Setup.exe`
2. The application starts with an icon in the system tray (no main window)
3. If the icon is not visible, expand the hidden tray area (arrow "Show hidden icons")

### Tray Menu Options
- **Light theme / Dark theme**: Instant theme switching
- **Schedule**: Enable/disable automatic switching
- **Light from... / Dark from...**: Set time in HH:mm format
- **Run at Windows startup**: Enable/disable auto-start
- **Language**: Russian, English, Français, Deutsch, Español
- **Monitor Diagnostics**: Generate detailed monitor report
- **Exit**: Close application

## 📁 Data Storage

- **Settings**: `%APPDATA%\AutoSwitchThemeW11\settings.json`
- **Logs**: `%APPDATA%\AutoSwitchThemeW11\app.log` (created on errors and startup events)
- **Diagnostic reports**: `%APPDATA%\AutoSwitchThemeW11\monitor_diagnostic.txt`

## 🔧 Command Line Parameters

- `-minimized` - Start minimized (used for auto-start)

## 🔄 Theme Switching - What the App Does

Writes to current user registry:
`HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize`
- DWord "AppsUseLightTheme": 1 - light, 0 - dark
- DWord "SystemUsesLightTheme": 1 - light, 0 - dark

### 🛡️ Safe Synchronization System (v1.5.1)
1. Write values to Windows registry
2. Safe system notifications:
   - WM_SETTINGCHANGE with 500ms timeout
   - 100ms pause
   - WM_THEMECHANGED with 500ms timeout
   - 100ms pause
   - WM_SYSCOLORCHANGE with 500ms timeout
   - Final 200ms pause for stabilization
3. Total delay ~1 second for maximum stability
4. No direct interference with Explorer.exe or taskbar windows

## 🗑️ Uninstallation

- Control Panel → Programs → Uninstall programs → Auto Switch Theme W11
- Or run installer and select Uninstall

## 🔍 Troubleshooting

1. Check if SmartScreen/antivirus blocks first launch
2. Look for icon in hidden tray area
3. Open log: `%APPDATA%\AutoSwitchThemeW11\app.log`
4. Check process in Task Manager: AutoSwitchThemeW11.exe
5. If no log and no process - run EXE directly from `publish/win-x64` folder
6. For schedule testing use: `.\test_schedule.ps1`
7. For monitor diagnostics use: `.\test_monitor_sync.ps1`
8. For detailed diagnostics use "Monitor Diagnostics" in context menu

## 📊 Performance Comparison

- **Version 1.4.0**: ~1500ms delay, aggressive methods, Explorer.exe crashes
- **Version 1.5.0**: <200ms delay, optimized methods, but unstable
- **Version 1.5.1**: ~1000ms delay, safe methods, complete stability
- **Improvement**: stability over speed, no Explorer.exe crashes

## 🏗️ Project Architecture

- **ThemeManager.cs** - Core theme switching logic (safe v1.5.1)
- **TrayAppManager.cs** - Tray icon management (optimized in v1.5.0)
- **TrayMenuFactory.cs** - Context menu creation (simplified in v1.5.0)
- **ScheduleManager.cs** - Schedule management
- **SettingsManager.cs** - Settings management
- **StartupManager.cs** - Auto-start management
- **AppLogger.cs** - Logging system
- **MonitorDiagnostics.cs** - Monitor diagnostics
- **App.xaml/cs** - Application entry point
- **MainWindow.xaml/cs** - Main window (unused)
- **TimeInputDialog.xaml/cs** - Time input dialog

## 📈 Version History

- **1.0.0-1.2.2**: Basic versions with simple synchronization
- **1.3.0**: Comprehensive solution with extended APIs
- **1.4.0**: Revolutionary solution with aggressive methods
- **1.5.0**: Fast and efficient solution with optimized architecture
- **1.5.1**: Critical security fixes, stability over speed

## 👨‍💻 Author

**@oh_johnny**
- GitHub: [https://github.com/oh_johnny](https://github.com/ohjohnnyoh/)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## ⚠️ Disclaimer

This application modifies Windows registry settings and system theme configurations. Use at your own risk. The author is not responsible for any system issues that may arise from using this software.
