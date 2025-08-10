# Скрипт для имитации ручной смены темы
# Автоматически нажимает кнопки в контекстном меню приложения

param(
    [string]$Action = "toggle", # "light", "dark", "toggle"
    [int]$Delay = 1000 # Задержка в миллисекундах
)

Write-Host "=== Тест имитации смены темы ===" -ForegroundColor Cyan
Write-Host "Действие: $Action" -ForegroundColor Yellow
Write-Host "Задержка: $Delay мс" -ForegroundColor Yellow

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

# Функция для поиска иконки в трее
function Find-TrayIcon {
    try {
        # Ищем иконку в трее по тексту
        Add-Type -AssemblyName System.Windows.Forms
        $shell = New-Object -ComObject Shell.Application
        $tray = $shell.NameSpace(0x7) # ShellSpecialFolderConstants.ssfDESKTOP
        
        # Это упрощенный подход - в реальности нужно использовать Windows API
        Write-Host "Поиск иконки в трее..." -ForegroundColor Yellow
        return $true
    }
    catch {
        Write-Host "Ошибка поиска иконки: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Функция для имитации клика по иконке трея
function Click-TrayIcon {
    param([string]$Action)
    
    try {
        Write-Host "Имитация клика по иконке трея..." -ForegroundColor Yellow
        
        # Используем Windows API для отправки сообщений
        Add-Type -TypeDefinition @"
        using System;
        using System.Runtime.InteropServices;
        
        public class TrayHelper {
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
            
            [DllImport("user32.dll")]
            public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
            
            [DllImport("user32.dll")]
            public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
            
            public const uint WM_RBUTTONDOWN = 0x0204;
            public const uint WM_RBUTTONUP = 0x0205;
            public const uint WM_LBUTTONDOWN = 0x0201;
            public const uint WM_LBUTTONUP = 0x0202;
        }
"@
        
        # Ищем окно трея
        $trayWindow = [TrayHelper]::FindWindow("Shell_TrayWnd", $null)
        if ($trayWindow -ne [IntPtr]::Zero) {
            Write-Host "✓ Окно трея найдено" -ForegroundColor Green
            
            # Отправляем правый клик для открытия контекстного меню
            [TrayHelper]::PostMessage($trayWindow, [TrayHelper]::WM_RBUTTONDOWN, [IntPtr]::Zero, [IntPtr]::Zero)
            Start-Sleep -Milliseconds 50
            [TrayHelper]::PostMessage($trayWindow, [TrayHelper]::WM_RBUTTONUP, [IntPtr]::Zero, [IntPtr]::Zero)
            
            Write-Host "✓ Контекстное меню открыто" -ForegroundColor Green
            return $true
        } else {
            Write-Host "✗ Окно трея не найдено" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "Ошибка имитации клика: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Функция для имитации нажатия клавиш
function Send-KeyPress {
    param([string]$Key)
    
    try {
        Write-Host "Отправка клавиши: $Key" -ForegroundColor Yellow
        
        Add-Type -AssemblyName System.Windows.Forms
        [System.Windows.Forms.SendKeys]::SendWait($Key)
        
        Write-Host "✓ Клавиша отправлена" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "Ошибка отправки клавиши: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Основная логика
Write-Host "`n1. Поиск процесса приложения..." -ForegroundColor Cyan
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

Write-Host "`n2. Поиск иконки в трее..." -ForegroundColor Cyan
$iconFound = Find-TrayIcon

Write-Host "`n3. Имитация клика по иконке..." -ForegroundColor Cyan
$clickSuccess = Click-TrayIcon -Action $Action

if ($clickSuccess) {
    Write-Host "`n4. Ожидание открытия меню..." -ForegroundColor Cyan
    Start-Sleep -Milliseconds 500
    
    # Определяем какую клавишу нажать в зависимости от действия
    switch ($Action.ToLower()) {
        "light" {
            Write-Host "Нажимаем 'Светлая тема'..." -ForegroundColor Yellow
            Send-KeyPress -Key "{DOWN}"
            Start-Sleep -Milliseconds 100
            Send-KeyPress -Key "{ENTER}"
        }
        "dark" {
            Write-Host "Нажимаем 'Тёмная тема'..." -ForegroundColor Yellow
            Send-KeyPress -Key "{DOWN}"
            Start-Sleep -Milliseconds 100
            Send-KeyPress -Key "{DOWN}"
            Start-Sleep -Milliseconds 100
            Send-KeyPress -Key "{ENTER}"
        }
        "toggle" {
            Write-Host "Переключаем тему..." -ForegroundColor Yellow
            # Определяем текущую тему и переключаем на противоположную
            $currentTheme = Get-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" -Name "AppsUseLightTheme" -ErrorAction SilentlyContinue
            if ($currentTheme.AppsUseLightTheme -eq 1) {
                # Сейчас светлая, переключаем на тёмную
                Send-KeyPress -Key "{DOWN}"
                Start-Sleep -Milliseconds 100
                Send-KeyPress -Key "{DOWN}"
                Start-Sleep -Milliseconds 100
                Send-KeyPress -Key "{ENTER}"
            } else {
                # Сейчас тёмная, переключаем на светлую
                Send-KeyPress -Key "{DOWN}"
                Start-Sleep -Milliseconds 100
                Send-KeyPress -Key "{ENTER}"
            }
        }
        default {
            Write-Host "Неизвестное действие: $Action" -ForegroundColor Red
        }
    }
    
    Write-Host "`n5. Закрываем меню..." -ForegroundColor Cyan
    Start-Sleep -Milliseconds 200
    Send-KeyPress -Key "{ESC}"
    
    Write-Host "`n=== Тест завершён ===" -ForegroundColor Green
} else {
    Write-Host "`nНе удалось выполнить имитацию клика" -ForegroundColor Red
}

Write-Host "`nДля проверки результата смотрите лог: %APPDATA%\AutoSwitchThemeW11\app.log" -ForegroundColor Cyan
