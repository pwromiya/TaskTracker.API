using System.Windows;
using TaskTracker.Application.Interfaces;
namespace TaskTracker.UI.Services;

// Service for changing application language at runtime
public class LanguageService : ILanguageService
{
    public void ChangeLanguage(string culture)
    {
        // Create ResourceDictionary for the specified language
        var dict = new ResourceDictionary
        {
            Source = new Uri(
                $"/Resources/Languages/Strings.{culture}.xaml", // Path to language resource file
                UriKind.Relative)
        };

        // Replace current language resources with new ones
        System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
        System.Windows.Application.Current.Resources.MergedDictionaries.Add(dict);
    }

    public string GetString(string key)
    {
        // Take a string from the application resources
        return System.Windows.Application.Current.Resources[key] as string ?? key;
    }
}