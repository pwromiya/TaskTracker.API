namespace TaskTracker.UI.Services;

// A service that provides localized strings from application resources
public static class LocalizationManager
{
    public static string GetString(string key)
    {
        if (System.Windows.Application.Current.Resources.Contains(key))
        {
            return System.Windows.Application.Current.Resources[key] as string ?? key;
        }
        return key;
    }
}