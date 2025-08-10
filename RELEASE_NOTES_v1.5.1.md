# Auto Switch Theme W11 v1.5.1

## 🇷🇺 Русский

### 🛡️ Критические исправления безопасности

**Версия 1.5.1** представляет собой важное обновление безопасности, направленное на устранение критических проблем стабильности системы.

#### 🔧 Основные исправления:
- **УСТРАНЕНЫ СБОИ EXPLORER.EXE** - полностью убраны агрессивные методы работы с окнами
- **УСТРАНЕНО МЕРЦАНИЕ ЭКРАНА** - безопасная синхронизация без прямого вмешательства в UI
- **БЕЗОПАСНАЯ АРХИТЕКТУРА** - только базовые системные уведомления
- **УВЕЛИЧЕНЫ ТАЙМАУТЫ** - 500мс вместо 50-100мс для максимальной стабильности
- **ДОБАВЛЕНЫ ПАУЗЫ** - 100-200мс между уведомлениями для предотвращения сбоев
- **СТАБИЛЬНОСТЬ ПРЕВЫШЕ СКОРОСТИ** - приоритет безопасности над производительностью

#### 🚀 Новые возможности:
- **Диагностика мониторов** - подробный анализ многомониторной конфигурации
- **Автоматическое обнаружение** всех мониторов и окон панели задач
- **Гарантированная синхронизация** на всех мониторах
- **Оптимизированная архитектура** для максимальной скорости

#### 📊 Сравнение производительности:
- **Версия 1.4.0**: ~1500мс задержка, агрессивные методы, сбои Explorer.exe
- **Версия 1.5.0**: <200мс задержка, оптимизированные методы, но нестабильно
- **Версия 1.5.1**: ~1000мс задержка, безопасные методы, полная стабильность

#### 🛡️ Безопасная система синхронизации:
1. Запись значений в реестр Windows
2. Безопасные системные уведомления:
   - WM_SETTINGCHANGE с таймаутом 500мс
   - Пауза 100мс
   - WM_THEMECHANGED с таймаутом 500мс
   - Пауза 100мс
   - WM_SYSCOLORCHANGE с таймаутом 500мс
   - Финальная пауза 200мс для стабилизации
3. Общая задержка ~1 секунда для максимальной стабильности
4. Никакого прямого вмешательства в Explorer.exe или окна панели задач

---

## 🇺🇸 English

### 🛡️ Critical Security Fixes

**Version 1.5.1** represents a crucial security update focused on resolving critical system stability issues.

#### 🔧 Key Fixes:
- **FIXED EXPLORER.EXE CRASHES** - completely removed aggressive window manipulation methods
- **ELIMINATED SCREEN FLICKERING** - safe synchronization without direct UI interference
- **SECURE ARCHITECTURE** - only basic system notifications
- **INCREASED TIMEOUTS** - 500ms instead of 50-100ms for maximum stability
- **ADDED PAUSES** - 100-200ms between notifications to prevent crashes
- **STABILITY OVER SPEED** - security priority over performance

#### 🚀 New Features:
- **Monitor diagnostics** - detailed multi-monitor configuration analysis
- **Automatic detection** of all monitors and taskbar windows
- **Guaranteed synchronization** across all monitors
- **Optimized architecture** for maximum performance

#### 📊 Performance Comparison:
- **Version 1.4.0**: ~1500ms delay, aggressive methods, Explorer.exe crashes
- **Version 1.5.0**: <200ms delay, optimized methods, but unstable
- **Version 1.5.1**: ~1000ms delay, safe methods, complete stability

#### 🛡️ Safe Synchronization System:
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

---

## 📦 Downloads

### Windows x64
- **Installer**: `AutoSwitchThemeW11-Setup.exe` (50.8 MB)
- **Portable**: `AutoSwitchThemeW11.exe` + dependencies

### System Requirements
- Windows 10/11 x64
- .NET 8 Runtime (included in installer)
- 100 MB free disk space

## 🔧 Installation

1. Download and run `AutoSwitchThemeW11-Setup.exe`
2. Follow the installation wizard
3. The application starts automatically in system tray
4. Right-click tray icon to access menu

## 🆘 Support

- **GitHub Issues**: [Report bugs or request features](https://github.com/ohjohnnyoh/AutoSwitchThemeW11/issues)
- **Documentation**: See README.md for detailed usage instructions
- **Logs**: Check `%APPDATA%\AutoSwitchThemeW11\app.log` for troubleshooting

---

**Author**: @ohjohnnyoh  
**License**: MIT  
**GitHub**: https://github.com/ohjohnnyoh/AutoSwitchThemeW11
