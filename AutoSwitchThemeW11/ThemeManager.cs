using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoSwitchThemeW11;

public sealed class ThemeManager
{
    private const string PersonalizePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
    private readonly AppLogger _logger = new();

    // Только базовые Windows API для безопасности
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam,
        uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

    // Константы
    private const uint HWND_BROADCAST = 0xffff;
    private const uint WM_SETTINGCHANGE = 0x001A;
    private const uint WM_THEMECHANGED = 0x031A;
    private const uint WM_SYSCOLORCHANGE = 0x0015;
    private const uint SMTO_ABORTIFHUNG = 0x0002;

    public void SetLightTheme()
    {
        _logger.Info("Setting light theme");
        SetThemeValues(1, 1);
    }

    public void SetDarkTheme()
    {
        _logger.Info("Setting dark theme");
        SetThemeValues(0, 0);
    }

    public bool IsLightTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(PersonalizePath);
            if (key == null) return true;

            var appsUseLight = key.GetValue("AppsUseLightTheme");
            return appsUseLight is int value && value == 1;
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to check theme", ex);
            return true;
        }
    }

    private void SetThemeValues(int appsUseLight, int systemUsesLight)
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(PersonalizePath);
            if (key == null)
            {
                _logger.Error("Failed to create registry key", new InvalidOperationException("Registry key creation failed"));
                return;
            }

            key.SetValue("AppsUseLightTheme", appsUseLight, RegistryValueKind.DWord);
            key.SetValue("SystemUsesLightTheme", systemUsesLight, RegistryValueKind.DWord);

            _logger.Info($"Theme values set: AppsUseLightTheme={appsUseLight}, SystemUsesLightTheme={systemUsesLight}");

            // Безопасная синхронизация без агрессивных методов
            PerformSafeSync();
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to set theme values", ex);
        }
    }

    private void PerformSafeSync()
    {
        try
        {
            _logger.Info("Starting safe theme synchronization");

            // Только базовые системные уведомления - безопасно для Explorer.exe
            SendSafeNotifications();

            _logger.Info("Safe theme synchronization completed");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to perform safe sync", ex);
        }
    }

    private void SendSafeNotifications()
    {
        try
        {
            _logger.Info("Sending safe system notifications");

            // Только базовые уведомления с увеличенными таймаутами для безопасности
            SendMessageTimeout(new IntPtr(unchecked((int)HWND_BROADCAST)), WM_SETTINGCHANGE, IntPtr.Zero,
                "ImmersiveColorSet", SMTO_ABORTIFHUNG, 500, out _);

            // Небольшая пауза между уведомлениями
            Thread.Sleep(100);

            SendMessageTimeout(new IntPtr(unchecked((int)HWND_BROADCAST)), WM_THEMECHANGED, IntPtr.Zero, "ThemeChanged",
                SMTO_ABORTIFHUNG, 500, out _);

            // Небольшая пауза между уведомлениями
            Thread.Sleep(100);

            SendMessageTimeout(new IntPtr(unchecked((int)HWND_BROADCAST)), WM_SYSCOLORCHANGE, IntPtr.Zero, "SysColorChange",
                SMTO_ABORTIFHUNG, 500, out _);

            // Финальная пауза для стабилизации
            Thread.Sleep(200);

            _logger.Info("Safe notifications sent successfully");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to send safe notifications", ex);
        }
    }
}




