# Продвинутый скрипт для тестирования расписания
# Детальная диагностика работы расписания

param(
    [string]$Action = "check", # "check", "test", "force", "simulate"
    [string]$Time = "" # Время для симуляции в формате HH:mm
)

Write-Host "=== Продвинутый тест расписания ===" -ForegroundColor Cyan
Write-Host "Действие: $Action" -ForegroundColor Yellow
if ($Time) { Write-Host "Время симуляции: $Time" -ForegroundColor Yellow }

# Функция для получения настроек из реестра
function Get-AppSettings {
    $settingsPath = "$env:APPDATA\AutoSwitchThemeW11\settings.json"
    if (Test-Path $settingsPath) {
        try {
            $json = Get-Content $settingsPath -Raw
            $settings = $json | ConvertFrom-Json
            return $settings
        }
        catch {
            Write-Host "Ошибка чтения настроек: $($_.Exception.Message)" -ForegroundColor Red
            return $null
        }
    } else {
        Write-Host "Файл настроек не найден: $settingsPath" -ForegroundColor Red
        return $null
    }
}

# Функция для получения текущей темы из реестра
function Get-CurrentTheme {
    try {
        $theme = Get-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" -Name "AppsUseLightTheme" -ErrorAction SilentlyContinue
        if ($theme) {
            return $theme.AppsUseLightTheme -eq 1
        } else {
            Write-Host "Не удалось получить тему из реестра" -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "Ошибка чтения темы: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Функция для проверки логики расписания
function Test-ScheduleLogic {
    param(
        [TimeSpan]$Now,
        [TimeSpan]$LightFrom,
        [TimeSpan]$DarkFrom
    )
    
    Write-Host "`n=== Тест логики расписания ===" -ForegroundColor Green
    Write-Host "Текущее время: $($Now.ToString('hh\:mm'))" -ForegroundColor White
    Write-Host "Светлая тема с: $($LightFrom.ToString('hh\:mm'))" -ForegroundColor White
    Write-Host "Тёмная тема с: $($DarkFrom.ToString('hh\:mm'))" -ForegroundColor White
    
    # Логика определения интервала
    if ($LightFrom -eq $DarkFrom) {
        Write-Host "Результат: ВСЕГДА СВЕТЛАЯ (времена равны)" -ForegroundColor Yellow
        return $true
    }
    
    if ($LightFrom -lt $DarkFrom) {
        # Без перехода через полночь
        $isLight = $Now -ge $LightFrom -and $Now -lt $DarkFrom
        Write-Host "Интервал: БЕЗ перехода через полночь" -ForegroundColor Cyan
        Write-Host "Условие: $Now >= $LightFrom И $Now < $DarkFrom" -ForegroundColor Gray
    } else {
        # С переходом через полночь
        $isLight = $Now -ge $LightFrom -or $Now -lt $DarkFrom
        Write-Host "Интервал: С переходом через полночь" -ForegroundColor Cyan
        Write-Host "Условие: $Now >= $LightFrom ИЛИ $Now < $DarkFrom" -ForegroundColor Gray
    }
    
    $themeText = if ($isLight) { "СВЕТЛАЯ" } else { "ТЁМНАЯ" }
    $color = if ($isLight) { "Yellow" } else { "Magenta" }
    Write-Host "Результат: $themeText" -ForegroundColor $color
    return $isLight
}

# Функция для анализа логов
function Analyze-Logs {
    $logPath = "$env:APPDATA\AutoSwitchThemeW11\app.log"
    if (-not (Test-Path $logPath)) {
        Write-Host "Лог файл не найден: $logPath" -ForegroundColor Red
        return
    }
    
    Write-Host "`n=== Анализ логов ===" -ForegroundColor Green
    
    try {
        $logs = Get-Content $logPath -Tail 50
        $scheduleLogs = $logs | Where-Object { $_ -match "Schedule" }
        $errorLogs = $logs | Where-Object { $_ -match "ERROR" }
        
        Write-Host "Последние записи расписания:" -ForegroundColor Cyan
        if ($scheduleLogs) {
            $scheduleLogs | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
        } else {
            Write-Host "  Записей расписания не найдено" -ForegroundColor Red
        }
        
        Write-Host "`nПоследние ошибки:" -ForegroundColor Cyan
        if ($errorLogs) {
            $errorLogs | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
        } else {
            Write-Host "  Ошибок не найдено" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "Ошибка чтения логов: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Функция для симуляции времени
function Simulate-Time {
    param([string]$Time)
    
    if (-not $Time) {
        Write-Host "Не указано время для симуляции" -ForegroundColor Red
        return
    }
    
    try {
        $simulatedTime = [TimeSpan]::Parse($Time)
        Write-Host "`n=== Симуляция времени $Time ===" -ForegroundColor Green
        
        $settings = Get-AppSettings
        if ($settings) {
            $lightFrom = [TimeSpan]::Parse($settings.LightFrom)
            $darkFrom = [TimeSpan]::Parse($settings.DarkFrom)
            
            $shouldBeLight = Test-ScheduleLogic -Now $simulatedTime -LightFrom $lightFrom -DarkFrom $darkFrom
            $currentTheme = Get-CurrentTheme
            
            Write-Host "`nРезультат симуляции:" -ForegroundColor Yellow
            $shouldBeText = if ($shouldBeLight) { "СВЕТЛАЯ" } else { "ТЁМНАЯ" }
            $shouldBeColor = if ($shouldBeLight) { "Yellow" } else { "Magenta" }
            $currentText = if ($currentTheme) { "СВЕТЛАЯ" } else { "ТЁМНАЯ" }
            Write-Host "Должна быть тема: $shouldBeText" -ForegroundColor $shouldBeColor
            Write-Host "Текущая тема: $currentText" -ForegroundColor White
            
            if ($shouldBeLight -eq $currentTheme) {
                Write-Host "✓ Тема соответствует расписанию" -ForegroundColor Green
            } else {
                Write-Host "✗ Тема НЕ соответствует расписанию!" -ForegroundColor Red
            }
        }
    }
    catch {
        Write-Host "Ошибка симуляции: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Основная логика
switch ($Action.ToLower()) {
    "check" {
        Write-Host "`n1. Проверка настроек..." -ForegroundColor Cyan
        $settings = Get-AppSettings
        if ($settings) {
            Write-Host "✓ Настройки загружены" -ForegroundColor Green
            Write-Host "Расписание включено: $($settings.EnableSchedule)" -ForegroundColor White
            Write-Host "Светлая тема с: $($settings.LightFrom)" -ForegroundColor White
            Write-Host "Тёмная тема с: $($settings.DarkFrom)" -ForegroundColor White
        }
        
        Write-Host "`n2. Проверка текущей темы..." -ForegroundColor Cyan
        $currentTheme = Get-CurrentTheme
        if ($currentTheme -ne $null) {
            $currentThemeText = if ($currentTheme) { "СВЕТЛАЯ" } else { "ТЁМНАЯ" }
            Write-Host "✓ Текущая тема: $currentThemeText" -ForegroundColor Green
        }
        
        Write-Host "`n3. Проверка процесса..." -ForegroundColor Cyan
        $process = Get-Process -Name "AutoSwitchThemeW11" -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "✓ Процесс запущен: PID $($process.Id)" -ForegroundColor Green
        } else {
            Write-Host "✗ Процесс не найден" -ForegroundColor Red
        }
        
        Write-Host "`n4. Тест логики расписания..." -ForegroundColor Cyan
        if ($settings) {
            $now = [DateTime]::Now.TimeOfDay
            $lightFrom = [TimeSpan]::Parse($settings.LightFrom)
            $darkFrom = [TimeSpan]::Parse($settings.DarkFrom)
            
            Test-ScheduleLogic -Now $now -LightFrom $lightFrom -DarkFrom $darkFrom
        }
        
        Analyze-Logs
    }
    
    "test" {
        Write-Host "`n=== Тестирование различных сценариев ===" -ForegroundColor Green
        
        $testCases = @(
            @{ Time = "06:00"; LightFrom = "07:00"; DarkFrom = "21:00"; Expected = $false },
            @{ Time = "08:00"; LightFrom = "07:00"; DarkFrom = "21:00"; Expected = $true },
            @{ Time = "22:00"; LightFrom = "07:00"; DarkFrom = "21:00"; Expected = $false },
            @{ Time = "23:00"; LightFrom = "21:00"; DarkFrom = "07:00"; Expected = $true },
            @{ Time = "06:00"; LightFrom = "21:00"; DarkFrom = "07:00"; Expected = $true },
            @{ Time = "08:00"; LightFrom = "21:00"; DarkFrom = "07:00"; Expected = $false }
        )
        
        foreach ($test in $testCases) {
            $now = [TimeSpan]::Parse($test.Time)
            $lightFrom = [TimeSpan]::Parse($test.LightFrom)
            $darkFrom = [TimeSpan]::Parse($test.DarkFrom)
            
            Write-Host "`nТест: $($test.Time) (светлая: $($test.LightFrom)-$($test.DarkFrom))" -ForegroundColor Cyan
            $result = Test-ScheduleLogic -Now $now -LightFrom $lightFrom -DarkFrom $darkFrom
            
            if ($result -eq $test.Expected) {
                Write-Host "✓ Тест пройден" -ForegroundColor Green
            } else {
                Write-Host "✗ Тест провален! Ожидалось: $($test.Expected), Получено: $result" -ForegroundColor Red
            }
        }
    }
    
    "simulate" {
        Simulate-Time -Time $Time
    }
    
    "force" {
        Write-Host "`n=== Принудительная проверка ===" -ForegroundColor Green
        
        $settings = Get-AppSettings
        if ($settings -and $settings.EnableSchedule) {
            Write-Host "Расписание включено, выполняем принудительную проверку..." -ForegroundColor Yellow
            
            # Здесь можно добавить вызов метода приложения для принудительной проверки
            Write-Host "Для принудительной проверки используйте контекстное меню приложения" -ForegroundColor Cyan
        } else {
            Write-Host "Расписание отключено" -ForegroundColor Red
        }
    }
    
    default {
        Write-Host "Неизвестное действие: $Action" -ForegroundColor Red
        Write-Host "Доступные действия: check, test, simulate, force" -ForegroundColor Yellow
    }
}

Write-Host "`n=== Тест завершён ===" -ForegroundColor Green
