using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TaskTracker.Application.Interfaces;
using TaskTracker.UI.Api;
using TaskTracker.UI.ViewModels;
using TaskTracker.UI.Views;

namespace TaskTracker.UI.Services;

// Window navigation service implementation
public class WindowService : IWindowService
{
    private Window _currentWindow;
    private Window _previousWindow;

    public void ShowMain()
    {
        var window = App.ServiceProvider.GetRequiredService<MainView>();

        var projectsVM = new ProjectsViewModel(
            projectService: App.ServiceProvider.GetRequiredService<IProjectApiService>(),
            messageService: App.ServiceProvider.GetRequiredService<IMessageService>()
        );

        var tasksVM = new TasksViewModel(
            taskService: App.ServiceProvider.GetRequiredService<ITaskApiService>(),
            messageService: App.ServiceProvider.GetRequiredService<IMessageService>()
        );

        var mainVM = new MainViewModel(
            projectsVM,
            tasksVM,
            windowService: this,
            errorHandler: App.ServiceProvider.GetRequiredService<IErrorHandler>(),
            userApiService: App.ServiceProvider.GetRequiredService<IUserApiService>(),
            messageService: App.ServiceProvider.GetRequiredService<IMessageService>(),
            authApiService: App.ServiceProvider.GetRequiredService<IAuthApiService>(),
            tokenStorage:   App.ServiceProvider.GetRequiredService<TokenStorage>()
        );

        window.DataContext = mainVM;

        _previousWindow = _currentWindow;
        _currentWindow = window;
        window.Show();
    }

    public void ShowRegister()
    {
        var window = App.ServiceProvider.GetRequiredService<RegisterView>();
        _previousWindow = _currentWindow;
        _currentWindow = window;
        window.Show();
    }

    public void ShowLogin()
    {
        var window = App.ServiceProvider.GetRequiredService<LoginView>();
        _previousWindow = _currentWindow;
        _currentWindow = window;
        window.Show();
    }

    public void CloseCurrent()
    {
        _currentWindow?.Close(); // Close current window if exists
    }

    public void ClosePrevious()
    {
        _previousWindow?.Close(); // Close previous wiщndow if exists
    }
}