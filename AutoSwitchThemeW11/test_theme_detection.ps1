# Скрипт для диагностики определения темы системы
Write-Host "=== ДИАГНОСТИКА ОПРЕДЕЛЕНИЯ ТЕМЫ СИСТЕМЫ ===" -ForegroundColor Yellow

# Получаем текущую тему из реестра
$regPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"
$appsUseLight = Get-ItemProperty -Path $regPath -Name "AppsUseLightTheme" -ErrorAction SilentlyContinue
$systemUsesLight = Get-ItemProperty -Path $regPath -Name "SystemUsesLightTheme" -ErrorAction SilentlyContinue

Write-Host "AppsUseLightTheme: $($appsUseLight.AppsUseLightTheme)" -ForegroundColor Cyan
Write-Host "SystemUsesLightTheme: $($systemUsesLight.SystemUsesLightTheme)" -ForegroundColor Cyan

# Определяем тему
$isLightTheme = $appsUseLight.AppsUseLightTheme -eq 1
$isDarkTheme = $appsUseLight.AppsUseLightTheme -eq 0

Write-Host "`n=== РЕЗУЛЬТАТ ОПРЕДЕЛЕНИЯ ===" -ForegroundColor Yellow
Write-Host "Светлая тема: $isLightTheme" -ForegroundColor Green
Write-Host "Тёмная тема: $isDarkTheme" -ForegroundColor Green

# Проверяем визуальное определение
Write-Host "`n=== ВИЗУАЛЬНАЯ ПРОВЕРКА ===" -ForegroundColor Yellow
Write-Host "Если AppsUseLightTheme = 1, то СВЕТЛАЯ тема" -ForegroundColor White
Write-Host "Если AppsUseLightTheme = 0, то ТЁМНАЯ тема" -ForegroundColor White

Write-Host "`n=== ТЕКУЩЕЕ СОСТОЯНИЕ ===" -ForegroundColor Yellow
if ($isLightTheme) {
    Write-Host "СИСТЕМА В СВЕТЛОЙ ТЕМЕ" -ForegroundColor Green
    Write-Host "Контекстное меню должно быть СВЕТЛЫМ" -ForegroundColor Green
} else {
    Write-Host "СИСТЕМА В ТЁМНОЙ ТЕМЕ" -ForegroundColor Green
    Write-Host "Контекстное меню должно быть ТЁМНЫМ" -ForegroundColor Green
}

Write-Host "`nНажмите любую клавишу для выхода..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
