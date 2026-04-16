using System.Windows;
using System.Windows.Controls;

namespace TaskTracker.UI.Views.UserControls;

// Language switcher UserControl
public partial class LanguageSwitcher : UserControl
{
    public LanguageSwitcher()
    {
        InitializeComponent();

        // Set DataContext when control loads (LanguageViewModel from app resources)
        this.Loaded += (s, e) =>
        {
            if (System.Windows.Application.Current.Resources.Contains("LangVM"))
            {
                this.DataContext = System.Windows.Application.Current.Resources["LangVM"];
            }
        };
    }
}