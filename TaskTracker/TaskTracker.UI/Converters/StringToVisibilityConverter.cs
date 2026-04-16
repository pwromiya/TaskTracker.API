using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskTracker.UI.Converters;

// Converts string to Visibility: Visible if string has content, Collapsed if empty/null
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if string is null or empty, collapse if true
        return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // One-way converter
        throw new NotImplementedException();
    }
}