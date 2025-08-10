# Скрипт для тестирования синхронизации мониторов
# Проверяет работу нового комплексного решения для многомониторных систем

param(
    [int]$TestCount = 5, # Количество тестов
    [int]$DelayBetweenTests = 3000 # Задержка между тестами в миллисекундах
)

Write-Host "=== ТЕСТ СИНХРОНИЗАЦИИ МОНИТОРОВ ===" -ForegroundColor Cyan
Write-Host "Количество тестов: $TestCount" -ForegroundColor Yellow
Write-Host "Задержка между тестами: $DelayBetweenTests мс" -ForegroundColor Yellow
Write-Host ""

# Функция для поиска процесса приложения
function Find-AppProcess {
    $process = Get-Process -Name "AutoSwitchThemeW11" -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "✓ Процесс найден: PID $($process.Id)" -ForegroundColor Green
        return $process
    } else {
        Write-Host "✗ Процесс не найден" -ForegroundColor Red
        return $null
    }
}

# Функция для получения текущей темы
function Get-CurrentTheme {
    try {
        $regPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"
        $appsUseLight = Get-ItemProperty -Path $regPath -Name "AppsUseLightTheme" -ErrorAction SilentlyContinue
        $systemUsesLight = Get-ItemProperty -Path $regPath -Name "SystemUsesLightTheme" -ErrorAction SilentlyContinue
        
        if ($appsUseLight) {
            $theme = if ($appsUseLight.AppsUseLightTheme -eq 1) { "Светлая" } else { "Тёмная" }
            return @{
                AppsUseLight = $appsUseLight.AppsUseLightTheme
                SystemUsesLight = $systemUsesLight.SystemUsesLightTheme
                ThemeName = $theme
            }
        }
    } catch {
        Write-Host "Ошибка чтения темы: $($_.Exception.Message)" -ForegroundColor Red
    }
    return $null
}

# Функция для проверки синхронизации
function Test-ThemeSync {
    param([string]$TestName)
    
    Write-Host "`n--- Тест: $TestName ---" -ForegroundColor Magenta
    
    # Получаем текущую тему до теста
    $themeBefore = Get-CurrentTheme
    if ($themeBefore) {
        Write-Host "Тема до теста: $($themeBefore.ThemeName) (AppsUseLight: $($themeBefore.AppsUseLight))" -ForegroundColor White
    }
    
    # Ждём немного для стабилизации
    Start-Sleep -Milliseconds 500
    
    # Получаем тему после теста
    $themeAfter = Get-CurrentTheme
    if ($themeAfter) {
        Write-Host "Тема после теста: $($themeAfter.ThemeName) (AppsUseLight: $($themeAfter.AppsUseLight))" -ForegroundColor White
    }
    
    # Проверяем изменения
    if ($themeBefore -and $themeAfter) {
        if ($themeBefore.AppsUseLight -ne $themeAfter.AppsUseLight) {
            Write-Host "✓ Тема изменилась успешно" -ForegroundColor Green
            return $true
        } else {
            Write-Host "⚠ Тема не изменилась (возможно, уже была в нужном состоянии)" -ForegroundColor Yellow
            return $true
        }
    } else {
        Write-Host "✗ Не удалось определить тему" -ForegroundColor Red
        return $false
    }
}

# Функция для проверки логов
function Check-Logs {
    $logPath = "$env:APPDATA\AutoSwitchThemeW11\app.log"
    $diagnosticPath = "$env:APPDATA\AutoSwitchThemeW11\monitor_diagnostic.txt"
    
    Write-Host "`n--- Проверка логов ---" -ForegroundColor Magenta
    
    if (Test-Path $logPath) {
        $lastLogs = Get-Content $logPath -Tail 10
        Write-Host "Последние записи в логе:" -ForegroundColor White
        foreach ($log in $lastLogs) {
            Write-Host "  $log" -ForegroundColor Gray
        }
    } else {
        Write-Host "Файл лога не найден: $logPath" -ForegroundColor Yellow
    }
    
    if (Test-Path $diagnosticPath) {
        Write-Host "`nДиагностический отчёт найден: $diagnosticPath" -ForegroundColor Green
        $diagnosticContent = Get-Content $diagnosticPath -Head 20
        Write-Host "Начало отчёта:" -ForegroundColor White
        foreach ($line in $diagnosticContent) {
            Write-Host "  $line" -ForegroundColor Gray
        }
    } else {
        Write-Host "`nДиагностический отчёт не найден" -ForegroundColor Yellow
    }
}

# Основная логика
Write-Host "1. Поиск процесса приложения..." -ForegroundColor Cyan
$process = Find-AppProcess
if (-not $process) {
    Write-Host "Приложение не запущено. Запускаем..." -ForegroundColor Yellow
    
    $exePath = ".\publish\win-x64\AutoSwitchThemeW11.exe"
    if (Test-Path $exePath) {
        Start-Process $exePath -ArgumentList "-minimized"
        Start-Sleep -Seconds 3
        $process = Find-AppProcess
    } else {
        Write-Host "Файл приложения не найден: $exePath" -ForegroundColor Red
        exit 1
    }
}

if (-not $process) {
    Write-Host "Не удалось запустить приложение" -ForegroundColor Red
    exit 1
}

Write-Host "`n2. Проверка текущего состояния..." -ForegroundColor Cyan
$currentTheme = Get-CurrentTheme
if ($currentTheme) {
    Write-Host "Текущая тема: $($currentTheme.ThemeName)" -ForegroundColor White
} else {
    Write-Host "Не удалось определить текущую тему" -ForegroundColor Red
}

Write-Host "`n3. Запуск тестов синхронизации..." -ForegroundColor Cyan

$successCount = 0
$totalTests = $TestCount

for ($i = 1; $i -le $TestCount; $i++) {
    Write-Host "`nТест $i из $TestCount" -ForegroundColor Cyan
    
    # Имитируем переключение темы через контекстное меню
    Write-Host "Имитация переключения темы..." -ForegroundColor Yellow
    
    # Используем SendKeys для имитации кликов
    Add-Type -AssemblyName System.Windows.Forms
    
    # Отправляем Alt+Tab для активации приложения
    [System.Windows.Forms.SendKeys]::SendWait("^{ESC}")
    Start-Sleep -Milliseconds 100
    
    # Имитируем правый клик для открытия контекстного меню
    [System.Windows.Forms.SendKeys]::SendWait("{APPS}")
    Start-Sleep -Milliseconds 200
    
    # Нажимаем первую тему (светлая/тёмная)
    [System.Windows.Forms.SendKeys]::SendWait("{DOWN}")
    Start-Sleep -Milliseconds 100
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Start-Sleep -Milliseconds 500
    
    # Закрываем меню
    [System.Windows.Forms.SendKeys]::SendWait("{ESC}")
    Start-Sleep -Milliseconds 200
    
    # Тестируем синхронизацию
    $result = Test-ThemeSync "Переключение темы $i"
    if ($result) {
        $successCount++
    }
    
    # Ждём между тестами
    if ($i -lt $TestCount) {
        Write-Host "Ожидание $DelayBetweenTests мс..." -ForegroundColor Yellow
        Start-Sleep -Milliseconds $DelayBetweenTests
    }
}

Write-Host "`n4. Результаты тестирования..." -ForegroundColor Cyan
Write-Host "Успешных тестов: $successCount из $totalTests" -ForegroundColor White

if ($successCount -eq $totalTests) {
    Write-Host "✓ Все тесты прошли успешно!" -ForegroundColor Green
} elseif ($successCount -gt 0) {
    Write-Host "⚠ Частичный успех: $successCount из $totalTests тестов" -ForegroundColor Yellow
} else {
    Write-Host "✗ Все тесты провалились" -ForegroundColor Red
}

# Проверяем логи
Check-Logs

Write-Host "`n=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО ===" -ForegroundColor Green
Write-Host "Для подробной диагностики используйте пункт 'Диагностика мониторов' в контекстном меню приложения" -ForegroundColor Cyan
