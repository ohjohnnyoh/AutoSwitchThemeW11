using System;
using System.IO;

namespace AutoSwitchThemeW11;

public sealed class AppLogger
{
    private readonly string _logPath;

    public AppLogger()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AutoSwitchThemeW11");
        Directory.CreateDirectory(dir);
        _logPath = Path.Combine(dir, "app.log");
    }

    public void Info(string message) => Write("INFO", message);
    public void Error(string message, Exception ex) => Write("ERROR", message + ": " + ex);

    private void Write(string level, string message)
    {
        try
        {
            File.AppendAllText(_logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}");
        }
        catch
        {
            // ignore logging failures
        }
    }
}



