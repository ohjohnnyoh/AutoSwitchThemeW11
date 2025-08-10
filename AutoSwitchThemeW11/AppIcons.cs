using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace AutoSwitchThemeW11;

public static class AppIcons
{
    public static System.Drawing.Icon GetAppIcon()
    {
        // Попытка загрузить встроенный ресурс Assets/app.ico, иначе возьмём иконку из сборки
        var asm = Assembly.GetExecutingAssembly();
        var resourceName = Array.Find(asm.GetManifestResourceNames(), n => n.EndsWith("app.ico", StringComparison.OrdinalIgnoreCase));
        if (resourceName != null)
        {
            using var stream = asm.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                return new Icon(stream);
            }
        }

        var exePath = Environment.ProcessPath ?? AppContext.BaseDirectory;
        return Icon.ExtractAssociatedIcon(exePath) ?? SystemIcons.Application;
    }
}


