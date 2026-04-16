using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskTracker.UI.Converters;

// Converts null values to Visibility for UI controls

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool invert = false;

        // Check for inversion parameter
        if (parameter != null && bool.TryParse(parameter.ToString(), out bool param))
        {
            invert = param;
        }

        bool isVisible = value != null;

        // Invert visibility if parameter requests it
        if (invert)
            isVisible = !isVisible;

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // One-way conversion only
        throw new NotImplementedException();
    }
}