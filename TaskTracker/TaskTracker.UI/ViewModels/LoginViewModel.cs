using System.ComponentModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.UI.Services;

namespace TaskTracker.UI.ViewModels;

// ViewModel for user login functionality (LoginView)
public class LoginViewModel : INotifyPropertyChanged
{
    //private readonly IUserService _userService; // The main logic service of this model
    private readonly IAuthApiService _authService;
    private readonly IWindowService _windowService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService;
    private readonly IErrorHandler _errorHandler;

    private string _login;
    public string Login
    {
        get => _login;
        set
        {
            _login = value;
            OnPropertyChanged(nameof(Login));
        }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel(
        //IUserService userService,
        IAuthApiService authService,
        IWindowService windowService,
        IMessageService messageService,
        ILanguageService languageService,
        IErrorHandler errorHandler)
    {
        //_userService = userService;
        _authService = authService;
        _windowService = windowService;
        _messageService = messageService;
        _languageService = languageService;
        _errorHandler = errorHandler;

        LoginCommand = new RelayCommand(async param => await LoginAsync(param?.ToString()));
    }

    // Perform login
    public async Task LoginAsync(string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
            {
                _messageService.ShowWarning(_languageService.GetString("EnterCredentials"));
                return;
            }

            //var user = await _userService.LoginAsync(Login, password);
            var result = await _authService.Login(Login, password);
            if (result != null)
            {
                _messageService.ShowInformation(
                    string.Format(_languageService.GetString("LoginSuccess"), result.Login, result.Id));
                _windowService.ShowMain();
                _windowService.ClosePrevious(); // Close login window
            }
            else
            {
                _messageService.ShowError(_languageService.GetString("InvalidLoginOrPassword"));
            }
        }
        catch (Exception ex)
        {
            _errorHandler.Handle(ex);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}