using System.ComponentModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.Commands;
using TaskTracker.UI.Services;
using TaskTracker.Application.ApiInterfaces;

namespace TaskTracker.UI.ViewModels;

// ViewModel for user registration functionality (RegisterView)

public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly IAuthApiService _authService;  // The main logic service of this model

    private readonly IWindowService _windowService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService;
    private readonly TokenStorage _tokenStorage;
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

    public ICommand RegisterCommand { get; }
    public ICommand OpenLoginCommand { get; }
    public ICommand SetLangRuCommand { get; }
    public ICommand SetLangEnCommand { get; }
    public ICommand SetLangBeCommand { get; }

    public RegisterViewModel(
        IAuthApiService authService,
        IWindowService windowService,
        IMessageService messageService,
        ILanguageService languageService,
        TokenStorage tokenStorage,
        IErrorHandler errorHandler)
    {
        _authService = authService;
        _windowService = windowService;
        _messageService = messageService;
        _languageService = languageService;
        _tokenStorage = tokenStorage;
        _errorHandler = errorHandler;

        RegisterCommand = new RelayCommand(async param => await RegisterAsync(param?.ToString()));
        OpenLoginCommand = new RelayCommand(_ =>
        {
            _windowService.ShowLogin();
            _windowService.ClosePrevious(); // Close register window
        });

        // Language commands
        SetLangRuCommand = new RelayCommand(_ => _languageService.ChangeLanguage("ru"));
        SetLangEnCommand = new RelayCommand(_ => _languageService.ChangeLanguage("en"));
        SetLangBeCommand = new RelayCommand(_ => _languageService.ChangeLanguage("be"));

    }

    // Perform registration
    public async Task RegisterAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(Login))
        {
            _messageService.ShowWarning(_languageService.GetString("EnterLogin"));
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _messageService.ShowWarning(_languageService.GetString("EnterPassword"));
            return;
        }

        try
        {
            var result = await _authService.Register(Login, password);

            if (result == null)
            {
                _messageService.ShowWarning("Registration failed");
                return;
            }
            _tokenStorage.Token = result.Token;
            _messageService.ShowInformation(
                string.Format(_languageService.GetString("RegistrationSuccess"), result.Login, result.Id));
            _windowService.ShowMain();
            _windowService.ClosePrevious();  // Close register window
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