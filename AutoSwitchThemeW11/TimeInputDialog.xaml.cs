using System;
using System.Globalization;
using System.Windows;

namespace AutoSwitchThemeW11;

public partial class TimeInputDialog : Window
{
    public TimeSpan Result { get; private set; }

    public TimeInputDialog(string title, TimeSpan initial)
    {
        InitializeComponent();

        var isRu = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru", StringComparison.OrdinalIgnoreCase);
        Title = isRu ? "Время" : "Time";
        TitleText.Text = title;
        OkButton.Content = isRu ? "ОК" : "OK";
        CancelButton.Content = isRu ? "Отмена" : "Cancel";

        TimeBox.Text = initial.ToString("hh\\:mm");

        // Принудительно обновляем цвета при создании диалога
        UpdateThemeColors();

        OkButton.Click += (_, __) =>
        {
            if (TimeSpan.TryParseExact(TimeBox.Text.Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out var time))
            {
                Result = time;
                DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show(isRu ? "Введите время в формате HH:mm" : "Enter time in HH:mm format");
            }
        };

        CancelButton.Click += (_, __) => DialogResult = false;
    }

    private void UpdateThemeColors()
    {
        try
        {
            // Принудительно обновляем цвета элементов
            Background = System.Windows.SystemColors.WindowBrush;
            TitleText.Foreground = System.Windows.SystemColors.WindowTextBrush;
            TimeBox.Background = System.Windows.SystemColors.WindowBrush;
            TimeBox.Foreground = System.Windows.SystemColors.WindowTextBrush;
            TimeBox.BorderBrush = System.Windows.SystemColors.ControlDarkBrush;
            OkButton.Background = System.Windows.SystemColors.ControlBrush;
            OkButton.Foreground = System.Windows.SystemColors.ControlTextBrush;
            CancelButton.Background = System.Windows.SystemColors.ControlBrush;
            CancelButton.Foreground = System.Windows.SystemColors.ControlTextBrush;
        }
        catch
        {
            // Игнорируем ошибки обновления цветов
        }
    }

    public static bool TryAskTime(Window? owner, TimeSpan current, out TimeSpan result)
    {
        var isRu = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru", StringComparison.OrdinalIgnoreCase);
        var title = isRu ? "Введите время (HH:mm)" : "Enter time (HH:mm)";
        var dlg = new TimeInputDialog(title, current) { Owner = owner };
        var ok = dlg.ShowDialog() == true;
        result = ok ? dlg.Result : current;
        return ok;
    }
}


