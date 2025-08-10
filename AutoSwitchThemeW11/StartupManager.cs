using Microsoft.Win32;
using System;

namespace AutoSwitchThemeW11;

public sealed class StartupManager
{
    private const string RunKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    private const string AppRunName = "AutoSwitchThemeW11";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        var value = key?.GetValue(AppRunName) as string;
        return !string.IsNullOrEmpty(value);
    }

    public void Enable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true) ??
                         Registry.CurrentUser.CreateSubKey(RunKeyPath);
        var exePath = Environment.ProcessPath ?? AppContext.BaseDirectory;
        key.SetValue(AppRunName, $"\"{exePath}\" -minimized");
    }

    public void Disable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
        key?.DeleteValue(AppRunName, throwOnMissingValue: false);
    }
}



