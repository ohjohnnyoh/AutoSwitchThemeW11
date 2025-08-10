using System;
using System.Windows.Forms;

namespace AutoSwitchThemeW11;

public static class TrayMenuFactory
{
    public static ContextMenuStrip CreateContextMenu(
        Action setLightTheme,
        Action setDarkTheme,
        Action toggleSchedule,
        Action setLightFrom,
        Action setDarkFrom,
        Action toggleRunAtStartup,
        Action switchLanguage,
        Action generateDiagnostics,
        Action exit)
    {
        var menu = new ContextMenuStrip();
        
        // Используем простой стиль для стабильности
        ApplySimpleStyle(menu);

        // Создаём элементы меню
        var lightItem = CreateSimpleItem(GetLocalizedLabel("Light theme", "Светлая тема", "Thème clair", "Helles Thema", "Tema claro"), (_, __) => setLightTheme());
        var darkItem = CreateSimpleItem(GetLocalizedLabel("Dark theme", "Тёмная тема", "Thème sombre", "Dunkles Thema", "Tema oscuro"), (_, __) => setDarkTheme());
        
        menu.Items.Add(lightItem);
        menu.Items.Add(darkItem);

        menu.Items.Add(new ToolStripSeparator());

        var scheduleItem = CreateSimpleItem(GetLocalizedLabel("Schedule", "Расписание", "Planifier", "Zeitplan", "Programar"), (_, __) => toggleSchedule());
        menu.Items.Add(scheduleItem);

        menu.Items.Add(CreateSimpleItem(GetLocalizedLabel("Light from...", "Светлая с...", "Clair à partir de...", "Hell ab...", "Claro desde..."), (_, __) => setLightFrom()));
        menu.Items.Add(CreateSimpleItem(GetLocalizedLabel("Dark from...", "Тёмная с...", "Sombre à partir de...", "Dunkel ab...", "Oscuro desde..."), (_, __) => setDarkFrom()));

        menu.Items.Add(new ToolStripSeparator());

        var startupItem = CreateSimpleItem(GetLocalizedLabel("Run at Windows startup", "Запуск с Windows", "Démarrer avec Windows", "Mit Windows starten", "Ejecutar al iniciar Windows"), (_, __) => toggleRunAtStartup());
        menu.Items.Add(startupItem);

        menu.Items.Add(CreateSimpleItem(GetLocalizedLabel("Language", "Язык", "Langue", "Sprache", "Idioma"), (_, __) => switchLanguage()));

        menu.Items.Add(new ToolStripSeparator());
        
        // Добавляем пункт диагностики
        menu.Items.Add(CreateSimpleItem(GetLocalizedLabel("Monitor Diagnostics", "Диагностика мониторов", "Diagnostic des moniteurs", "Monitor-Diagnose", "Diagnóstico de monitores"), (_, __) => generateDiagnostics()));

        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(CreateSimpleItem(GetLocalizedLabel("Exit", "Выход", "Quitter", "Beenden", "Salir"), (_, __) => exit()));

        return menu;
    }

    private static void ApplySimpleStyle(ContextMenuStrip menu)
    {
        try
        {
            // Простые системные цвета для стабильности
            menu.BackColor = System.Drawing.SystemColors.Menu;
            menu.ForeColor = System.Drawing.SystemColors.MenuText;
            
            // Базовые настройки
            menu.ShowImageMargin = false;
            menu.ShowCheckMargin = false;
            menu.DropShadowEnabled = true;
        }
        catch
        {
            // Fallback к системным цветам
            menu.BackColor = System.Drawing.SystemColors.Menu;
            menu.ForeColor = System.Drawing.SystemColors.MenuText;
        }
    }

    private static ToolStripMenuItem CreateSimpleItem(string text, EventHandler onClick)
    {
        return new ToolStripMenuItem(text, null, onClick)
        {
            AutoSize = true
        };
    }

    private static string GetLocalizedLabel(string en, string ru, string? fr = null, string? de = null, string? es = null)
    {
        var currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
        
        return currentLanguage switch
        {
            "ru" => ru,
            "fr" => fr ?? en,
            "de" => de ?? en,
            "es" => es ?? en,
            _ => en
        };
    }
}


