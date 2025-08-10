# Улучшенный тестовый скрипт для проверки работы расписания
# Запускает приложение и проверяет логи

param(
    [string]$LogPath = "$env:APPDATA\AutoSwitchThemeW11\app.log"
)

Write-Host "=== Тест расписания AutoSwitchThemeW11 (Улучшенная версия) ===" -ForegroundColor Green

# Проверяем, запущено ли приложение
$process = Get-Process -Name "AutoSwitchThemeW11" -ErrorAction SilentlyContinue
if ($process) {
    Write-Host "Приложение уже запущено (PID: $($process.Id))" -ForegroundColor Yellow
} else {
    Write-Host "Приложение не запущено" -ForegroundColor Red
    Write-Host "Запустите приложение и повторите тест" -ForegroundColor Red
    exit 1
}

# Показываем текущие настройки
Write-Host "`n=== Текущие настройки ===" -ForegroundColor Cyan
$settingsPath = "$env:APPDATA\AutoSwitchThemeW11\settings.json"
if (Test-Path $settingsPath) {
    $settings = Get-Content $settingsPath | ConvertFrom-Json
    Write-Host "Расписание включено: $($settings.EnableSchedule)" -ForegroundColor White
    Write-Host "Светлая тема с: $($settings.LightFrom)" -ForegroundColor White
    Write-Host "Тёмная тема с: $($settings.DarkFrom)" -ForegroundColor White
} else {
    Write-Host "Файл настроек не найден" -ForegroundColor Red
}

# Показываем текущую тему в реестре
Write-Host "`n=== Текущая тема в реестре ===" -ForegroundColor Cyan
try {
    $regPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"
    $appsUseLight = Get-ItemProperty -Path $regPath -Name "AppsUseLightTheme" -ErrorAction SilentlyContinue
    $systemUsesLight = Get-ItemProperty -Path $regPath -Name "SystemUsesLightTheme" -ErrorAction SilentlyContinue
    
    if ($appsUseLight) {
        $theme = if ($appsUseLight.AppsUseLightTheme -eq 1) { "Светлая" } else { "Тёмная" }
        Write-Host "AppsUseLightTheme: $($appsUseLight.AppsUseLightTheme) ($theme)" -ForegroundColor White
    }
    if ($systemUsesLight) {
        Write-Host "SystemUsesLightTheme: $($systemUsesLight.SystemUsesLightTheme)" -ForegroundColor White
    }
} catch {
    Write-Host "Ошибка чтения реестра: $($_.Exception.Message)" -ForegroundColor Red
}

# Показываем последние записи в логе
Write-Host "`n=== Последние записи в логе ===" -ForegroundColor Cyan
if (Test-Path $LogPath) {
    $lastLogs = Get-Content $LogPath -Tail 15
    foreach ($log in $lastLogs) {
        if ($log -match "ERROR") {
            Write-Host $log -ForegroundColor Red
        } elseif ($log -match "Schedule") {
            Write-Host $log -ForegroundColor Yellow
        } else {
            Write-Host $log -ForegroundColor Gray
        }
    }
} else {
    Write-Host "Файл лога не найден" -ForegroundColor Yellow
}

# Проверяем логику расписания
Write-Host "`n=== Проверка логики расписания ===" -ForegroundColor Cyan
$currentTime = Get-Date
$currentTimeOfDay = $currentTime.TimeOfDay
Write-Host "Текущее время: $($currentTimeOfDay.ToString('hh\:mm'))" -ForegroundColor White

if (Test-Path $settingsPath) {
    $settings = Get-Content $settingsPath | ConvertFrom-Json
    $lightFrom = [TimeSpan]::Parse($settings.LightFrom)
    $darkFrom = [TimeSpan]::Parse($settings.DarkFrom)
    
    Write-Host "Интервал светлой темы: $($lightFrom.ToString('hh\:mm')) - $($darkFrom.ToString('hh\:mm'))" -ForegroundColor White
    
    # Проверяем логику
    $isLightTime = $false
    if ($lightFrom -eq $darkFrom) {
        $isLightTime = $true
        Write-Host "Логика: весь день светлая тема" -ForegroundColor Green
    } elseif ($lightFrom -lt $darkFrom) {
        $isLightTime = $currentTimeOfDay -ge $lightFrom -and $currentTimeOfDay -lt $darkFrom
        Write-Host "Логика: без перехода через полночь" -ForegroundColor Green
    } else {
        $isLightTime = $currentTimeOfDay -ge $lightFrom -or $currentTimeOfDay -lt $darkFrom
        Write-Host "Логика: с переходом через полночь" -ForegroundColor Green
    }
    
    Write-Host "Должна быть светлая тема: $isLightTime" -ForegroundColor White
}

Write-Host "`n=== Рекомендации ===" -ForegroundColor Green
Write-Host "1. Проверьте, что расписание включено в контекстном меню трея" -ForegroundColor White
Write-Host "2. Убедитесь, что время переключения настроено корректно" -ForegroundColor White
Write-Host "3. Подождите 30 секунд для следующей проверки расписания" -ForegroundColor White
Write-Host "4. Проверьте лог на наличие ошибок" -ForegroundColor White

Write-Host "`nДля мониторинга лога в реальном времени используйте:" -ForegroundColor Yellow
Write-Host "Get-Content '$LogPath' -Wait" -ForegroundColor Cyan
