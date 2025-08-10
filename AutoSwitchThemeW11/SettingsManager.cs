using System;
using System.IO;
using System.Text.Json;

namespace AutoSwitchThemeW11;

public sealed class Settings
{
    public bool EnableSchedule { get; set; } = true;
    public TimeSpan LightFrom { get; set; } = new TimeSpan(7, 0, 0);
    public TimeSpan DarkFrom { get; set; } = new TimeSpan(21, 0, 0);
}

public sealed class SettingsManager
{
    private readonly string _dirPath;
    private readonly string _filePath;

    public Settings Current { get; private set; } = new();

    public SettingsManager()
    {
        _dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AutoSwitchThemeW11");
        _filePath = Path.Combine(_dirPath, "settings.json");
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<Settings>(json);
                if (loaded != null)
                    Current = loaded;
            }
        }
        catch
        {
            // ignore and use defaults
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(_dirPath);
        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}



