using System.Windows;
using TaskTracker.UI.ViewModels;

namespace TaskTracker.UI.Views
{
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.LoginAsync(PasswordBox.Password);

                PasswordBox.Clear();
            }
        }
    }
}