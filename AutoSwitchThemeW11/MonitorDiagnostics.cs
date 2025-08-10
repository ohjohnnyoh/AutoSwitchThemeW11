using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Linq;

namespace AutoSwitchThemeW11;

public static class MonitorDiagnostics
{
    // Windows API для диагностики
    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    // Делегаты
    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    // Структуры
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MONITORINFO
    {
        public uint cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
    }

    // Класс для хранения информации о мониторе
    public class MonitorInfo
    {
        public IntPtr Handle { get; set; }
        public RECT Bounds { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public List<WindowInfo> TrayWindows { get; set; } = new();
    }

    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string WindowText { get; set; } = string.Empty;
        public RECT Bounds { get; set; }
        public bool IsVisible { get; set; }
    }

    private static readonly List<MonitorInfo> _monitors = new();
    private static readonly AppLogger _logger = new();

    public static string GenerateDiagnosticReport()
    {
        try
        {
            _logger.Info("Generating monitor diagnostic report");
            
            var report = new StringBuilder();
            report.AppendLine("=== ДИАГНОСТИЧЕСКИЙ ОТЧЁТ МОНИТОРОВ ===");
            report.AppendLine($"Время: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();

            // Обновляем информацию о мониторах
            RefreshMonitorInfo();

            // Информация о мониторах
            report.AppendLine($"Найдено мониторов: {_monitors.Count}");
            report.AppendLine();

            foreach (var monitor in _monitors)
            {
                report.AppendLine($"Монитор: {monitor.DeviceName}");
                report.AppendLine($"  Область: ({monitor.Bounds.left},{monitor.Bounds.top}) - ({monitor.Bounds.right},{monitor.Bounds.bottom})");
                report.AppendLine($"  Размер: {monitor.Bounds.right - monitor.Bounds.left} x {monitor.Bounds.bottom - monitor.Bounds.top}");
                report.AppendLine($"  Окон панели задач: {monitor.TrayWindows.Count}");
                
                foreach (var window in monitor.TrayWindows)
                {
                    report.AppendLine($"    - {window.ClassName}: \"{window.WindowText}\"");
                    report.AppendLine($"      Видимое: {window.IsVisible}");
                    report.AppendLine($"      Позиция: ({window.Bounds.left},{window.Bounds.top}) - ({window.Bounds.right},{window.Bounds.bottom})");
                }
                report.AppendLine();
            }

            // Дополнительная информация о системе
            report.AppendLine("=== ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ ===");
            report.AppendLine($"Операционная система: {Environment.OSVersion}");
            report.AppendLine($"Версия .NET: {Environment.Version}");
            report.AppendLine($"Количество процессоров: {Environment.ProcessorCount}");
            report.AppendLine();

            // Информация о теме
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", writable: false);
                var appsUseLight = key?.GetValue("AppsUseLightTheme") as int? ?? 1;
                var systemUsesLight = key?.GetValue("SystemUsesLightTheme") as int? ?? 1;
                
                report.AppendLine("=== ТЕКУЩАЯ ТЕМА ===");
                report.AppendLine($"AppsUseLightTheme: {appsUseLight} ({(appsUseLight == 1 ? "Светлая" : "Тёмная")})");
                report.AppendLine($"SystemUsesLightTheme: {systemUsesLight} ({(systemUsesLight == 1 ? "Светлая" : "Тёмная")})");
                report.AppendLine();
            }
            catch (Exception ex)
            {
                report.AppendLine($"Ошибка чтения темы: {ex.Message}");
                report.AppendLine();
            }

            _logger.Info("Monitor diagnostic report generated successfully");
            return report.ToString();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to generate diagnostic report", ex);
            return $"Ошибка генерации отчёта: {ex.Message}";
        }
    }

    private static void RefreshMonitorInfo()
    {
        try
        {
            _monitors.Clear();
            
            // Перечисляем все мониторы
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumCallback, IntPtr.Zero);
            
            // Находим окна панели задач для каждого монитора
            foreach (var monitor in _monitors)
            {
                FindTrayWindowsForMonitor(monitor);
            }

            _logger.Info($"Found {_monitors.Count} monitors with {_monitors.Sum(m => m.TrayWindows.Count)} tray windows total");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to refresh monitor info", ex);
        }
    }

    private static bool MonitorEnumCallback(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
    {
        try
        {
            var monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = (uint)Marshal.SizeOf(monitorInfo);
            
            if (GetMonitorInfo(hMonitor, ref monitorInfo))
            {
                var monitor = new MonitorInfo
                {
                    Handle = hMonitor,
                    Bounds = monitorInfo.rcMonitor,
                    DeviceName = monitorInfo.szDevice
                };
                
                _monitors.Add(monitor);
                _logger.Info($"Monitor found: {monitor.DeviceName} at ({monitor.Bounds.left},{monitor.Bounds.top})-({monitor.Bounds.right},{monitor.Bounds.bottom})");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error in monitor enum callback", ex);
        }
        
        return true; // Продолжаем перечисление
    }

    private static void FindTrayWindowsForMonitor(MonitorInfo monitor)
    {
        try
        {
            // Список известных классов окон панели задач
            var trayWindowClasses = new[]
            {
                "Shell_TrayWnd",
                "Shell_SecondaryTrayWnd",
                "Shell_TrayWnd_Secondary",
                "Shell_TrayWnd_Extended"
            };

            // Перечисляем все окна и ищем панели задач
            EnumWindows(WindowEnumCallback, IntPtr.Zero);

            // Функция обратного вызова для перечисления окон
            bool WindowEnumCallback(IntPtr hWnd, IntPtr lParam)
            {
                try
                {
                    var className = new StringBuilder(256);
                    GetClassName(hWnd, className, className.Capacity);
                    var windowClass = className.ToString();
                    
                    // Проверяем, является ли окно панелью задач
                    if (trayWindowClasses.Contains(windowClass))
                    {
                        var windowText = new StringBuilder(256);
                        GetWindowText(hWnd, windowText, windowText.Capacity);
                        
                        GetWindowRect(hWnd, out var windowRect);
                        var isVisible = IsWindowVisible(hWnd);
                        
                        var windowInfo = new WindowInfo
                        {
                            Handle = hWnd,
                            ClassName = windowClass,
                            WindowText = windowText.ToString(),
                            Bounds = windowRect,
                            IsVisible = isVisible
                        };
                        
                        // Проверяем, находится ли окно на данном мониторе
                        if (IsWindowOnMonitor(hWnd, monitor))
                        {
                            monitor.TrayWindows.Add(windowInfo);
                            _logger.Info($"Found tray window: {windowClass} on monitor {monitor.DeviceName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error in window enum callback", ex);
                }
                
                return true; // Продолжаем перечисление
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to find tray windows for monitor {monitor.DeviceName}", ex);
        }
    }

    private static bool IsWindowOnMonitor(IntPtr hWnd, MonitorInfo monitor)
    {
        try
        {
            GetWindowRect(hWnd, out var windowRect);
            
            // Проверяем, пересекается ли окно с областью монитора
            return !(windowRect.right < monitor.Bounds.left || 
                    windowRect.left > monitor.Bounds.right || 
                    windowRect.bottom < monitor.Bounds.top || 
                    windowRect.top > monitor.Bounds.bottom);
        }
        catch
        {
            return false;
        }
    }

    public static void SaveDiagnosticReportToFile()
    {
        try
        {
            var report = GenerateDiagnosticReport();
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AutoSwitchThemeW11",
                "monitor_diagnostic.txt");
            
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.WriteAllText(logPath, report, Encoding.UTF8);
            
            _logger.Info($"Diagnostic report saved to: {logPath}");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to save diagnostic report", ex);
        }
    }
}
