using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace AutoSwitchThemeW11;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private TrayAppManager? _trayAppManager;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Язык UI можно будет переключать из контекстного меню
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture;

        _trayAppManager = new TrayAppManager();
        _trayAppManager.Initialize(e.Args);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayAppManager?.Dispose();
        base.OnExit(e);
    }
}

