using System.Windows;
using TaskTracker.UI.ViewModels;

namespace TaskTracker.UI.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView(RegisterViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                await vm.RegisterAsync(PasswordBox.Password);
                PasswordBox.Clear();
            }
        }

    }
}