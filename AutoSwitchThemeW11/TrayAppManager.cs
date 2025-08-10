using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace AutoSwitchThemeW11;

public sealed class TrayAppManager : IDisposable
{
    private NotifyIcon? _trayIcon;
    private readonly SettingsManager _settingsManager = new();
    private readonly ThemeManager _themeManager = new();
    private readonly ScheduleManager _scheduleManager;
    private readonly StartupManager _startupManager = new();
    private readonly AppLogger _logger = new();
    private bool _disposed;

    public TrayAppManager()
    {
        _scheduleManager = new ScheduleManager(_themeManager, _settingsManager, _logger);
    }

    public void Initialize(string[] args)
    {
        try
        {
            _logger.Info("App starting");
            _settingsManager.Load();

            if (args is { Length: > 0 } && Array.Exists(args, a => a.Equals("-minimized", StringComparison.OrdinalIgnoreCase)))
            {
                // start minimized (no window anyway)
            }

            CreateTrayIcon();

            if (_settingsManager.Current.EnableSchedule)
            {
                _scheduleManager.Start();
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Initialize failed", ex);
            System.Windows.MessageBox.Show(ex.Message, "AutoSwitchThemeW11", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CreateTrayIcon()
    {
        try
        {
            _trayIcon = new NotifyIcon
            {
                Text = "Auto Switch Theme W11",
                Icon = AppIcons.GetAppIcon(),
                Visible = true
            };

            // Добавляем обработчик двойного клика для отладки
            _trayIcon.DoubleClick += (sender, e) =>
            {
                _logger.Info("Tray icon double-clicked");
                // Можно добавить показ информации о статусе
            };

            UpdateTrayMenu();
            
            _logger.Info("Tray icon created successfully");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to create tray icon", ex);
            throw;
        }
    }

    private void UpdateTrayMenu()
    {
        try
        {
            if (_trayIcon != null)
            {
                _logger.Info("Creating context menu...");
                
                var menu = TrayMenuFactory.CreateContextMenu(
                    setLightTheme: () => 
                    {
                        _logger.Info("Light theme selected");
                        _themeManager.SetLightTheme();
                    },
                    setDarkTheme: () => 
                    {
                        _logger.Info("Dark theme selected");
                        _themeManager.SetDarkTheme();
                    },
                    toggleSchedule: ToggleSchedule,
                    setLightFrom: SetLightFrom,
                    setDarkFrom: SetDarkFrom,
                    toggleRunAtStartup: ToggleRunAtStartup,
                    switchLanguage: SwitchLanguage,
                    generateDiagnostics: GenerateDiagnostics,
                    exit: ExitApp
                );

                _trayIcon.ContextMenuStrip = menu;
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to update tray menu", ex);
        }
    }

    private void ToggleSchedule()
    {
        try
        {
            _settingsManager.Current.EnableSchedule = !_settingsManager.Current.EnableSchedule;
            _settingsManager.Save();

            if (_settingsManager.Current.EnableSchedule)
            {
                _scheduleManager.Start();
            }
            else
            {
                _scheduleManager.Stop();
            }

            UpdateTrayMenu();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to toggle schedule", ex);
        }
    }

    private void SetLightFrom()
    {
        try
        {
            if (TimeInputDialog.TryAskTime(System.Windows.Application.Current.MainWindow, _settingsManager.Current.LightFrom, out var time))
            {
                _settingsManager.Current.LightFrom = time;
                _settingsManager.Save();
                UpdateTrayMenu();
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to set light theme time", ex);
        }
    }

    private void SetDarkFrom()
    {
        try
        {
            if (TimeInputDialog.TryAskTime(System.Windows.Application.Current.MainWindow, _settingsManager.Current.DarkFrom, out var time))
            {
                _settingsManager.Current.DarkFrom = time;
                _settingsManager.Save();
                UpdateTrayMenu();
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to set dark theme time", ex);
        }
    }

    private void ToggleRunAtStartup()
    {
        try
        {
            if (_startupManager.IsEnabled())
            {
                _startupManager.Disable();
            }
            else
            {
                _startupManager.Enable();
            }

            UpdateTrayMenu();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to toggle startup", ex);
        }
    }

    private void SwitchLanguage()
    {
        try
        {
            var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var languages = new[] { "ru", "en", "fr", "de", "es" };
            var currentIndex = Array.IndexOf(languages, currentLanguage);
            var nextIndex = (currentIndex + 1) % languages.Length;
            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(languages[nextIndex]);
            
            UpdateTrayMenu();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to switch language", ex);
        }
    }

    private void ExitApp() => System.Windows.Application.Current.Shutdown();

    private void GenerateDiagnostics()
    {
        try
        {
            _logger.Info("Generating monitor diagnostics");
            
            var report = MonitorDiagnostics.GenerateDiagnosticReport();
            var reportPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AutoSwitchThemeW11",
                "monitor_diagnostic.txt"
            );

            // Создаем директорию если не существует
            var directory = Path.GetDirectoryName(reportPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(reportPath, report);
            
            System.Windows.MessageBox.Show(
                $"Диагностический отчёт сохранён в:\n{reportPath}",
                "Monitor Diagnostics",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to generate diagnostics", ex);
            System.Windows.MessageBox.Show(
                $"Ошибка при создании отчёта: {ex.Message}",
                "Monitor Diagnostics",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            _scheduleManager?.Stop();
            _trayIcon?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.Error("Error during disposal", ex);
        }
        finally
        {
            _disposed = true;
        }
    }
}


