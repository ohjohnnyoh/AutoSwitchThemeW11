using System;
using System.Timers;
using TimersTimer = System.Timers.Timer;

namespace AutoSwitchThemeW11;

public sealed class ScheduleManager : IDisposable
{
    private readonly ThemeManager _themeManager;
    private readonly SettingsManager _settingsManager;
    private readonly AppLogger _logger;
    private readonly TimersTimer _timer;
    private bool _isLightTheme = true; // Текущее состояние темы

    public ScheduleManager(ThemeManager themeManager, SettingsManager settingsManager, AppLogger logger)
    {
        _themeManager = themeManager;
        _settingsManager = settingsManager;
        _logger = logger;
        _timer = new TimersTimer(5_000); // Проверяем каждые 5 секунд для максимальной точности
        _timer.Elapsed += OnTick;
    }

    public void Start()
    {
        try
        {
            // Определяем текущую тему при запуске
            _isLightTheme = _themeManager.IsLightTheme();
            _logger.Info($"Schedule started. Current theme: {(_isLightTheme ? "Light" : "Dark")}");
            
            // Принудительно проверяем и применяем правильную тему при запуске
            ForceCheckAndApplyTheme();
            
            _timer.Start();
            _logger.Info("Schedule timer started successfully");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to start schedule", ex);
        }
    }

    private void ForceCheckAndApplyTheme()
    {
        try
        {
            var now = DateTime.Now.TimeOfDay;
            var lightFrom = _settingsManager.Current.LightFrom;
            var darkFrom = _settingsManager.Current.DarkFrom;

            bool shouldBeLight = IsTimeInInterval(now, lightFrom, darkFrom);
            
            _logger.Info($"Force check: {now:hh\\:mm}, Should be light: {shouldBeLight}, Current is light: {_isLightTheme}");

            if (shouldBeLight && !_isLightTheme)
            {
                _logger.Info("Force switching to Light theme");
                _themeManager.SetLightTheme();
                _isLightTheme = true;
            }
            else if (!shouldBeLight && _isLightTheme)
            {
                _logger.Info("Force switching to Dark theme");
                _themeManager.SetDarkTheme();
                _isLightTheme = false;
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Force check failed", ex);
        }
    }

    public void Stop() 
    {
        _timer.Stop();
        _logger.Info("Schedule timer stopped");
    }

    public void ForceCheck()
    {
        _logger.Info("Force schedule check requested");
        OnTick(null, null!);
    }

    private void OnTick(object? sender, ElapsedEventArgs e)
    {
        try
        {
            // Проверяем, включено ли расписание
            if (!_settingsManager.Current.EnableSchedule)
            {
                _logger.Info("Schedule is disabled, skipping check");
                return;
            }

            var now = DateTime.Now.TimeOfDay;
            var lightFrom = _settingsManager.Current.LightFrom;
            var darkFrom = _settingsManager.Current.DarkFrom;

            // Интервалы могут пересекать полночь
            bool shouldBeLight = IsTimeInInterval(now, lightFrom, darkFrom);

            // Обновляем текущее состояние темы из системы
            bool currentSystemIsLight = _themeManager.IsLightTheme();
            if (currentSystemIsLight != _isLightTheme)
            {
                _logger.Info($"Theme state changed externally: {(_isLightTheme ? "Light" : "Dark")} -> {(_isLightTheme ? "Dark" : "Light")}");
                _isLightTheme = currentSystemIsLight;
            }

            // Логируем состояние для диагностики
            _logger.Info($"Schedule check: {now:hh\\:mm}, Light: {lightFrom:hh\\:mm}-{darkFrom:hh\\:mm}, Should be light: {shouldBeLight}, Current is light: {_isLightTheme}");

            // Переключаем тему только если она должна измениться
            if (shouldBeLight && !_isLightTheme)
            {
                _logger.Info($"Schedule: switching to Light theme at {now:hh\\:mm}");
                _themeManager.SetLightTheme();
                _isLightTheme = true;
            }
            else if (!shouldBeLight && _isLightTheme)
            {
                _logger.Info($"Schedule: switching to Dark theme at {now:hh\\:mm}");
                _themeManager.SetDarkTheme();
                _isLightTheme = false;
            }
            else
            {
                _logger.Info($"Schedule: no change needed at {now:hh\\:mm}");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Schedule tick failed", ex);
        }
    }

    private static bool IsTimeInInterval(TimeSpan now, TimeSpan start, TimeSpan end)
    {
        if (start == end)
        {
            // весь день светлая
            return true;
        }

        if (start < end)
        {
            // без перехода через полночь
            return now >= start && now < end;
        }
        else
        {
            // переход через полночь
            return now >= start || now < end;
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}


