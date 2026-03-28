using System.Windows;
using System.Windows.Controls;
using TaskTracker.UI.ViewModels;

namespace TaskTracker.UI.Views.UserControls;

public partial class EditProjectControl : UserControl
{
    public EditProjectControl()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.ProjectsVM.IsAddProjectPopupOpen = false;
    }
}
