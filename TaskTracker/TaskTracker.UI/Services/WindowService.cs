using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.Application.Services;
using TaskTracker.UI.ViewModels;
using TaskTracker.UI.Views;
using System;

namespace TaskTracker.UI.Services;

// Window navigation service implementation
public class WindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;
    private Window? _currentWindow;
    private Window? _previousWindow;

    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void ShowMain()
    {
        var window = _serviceProvider.GetRequiredService<MainView>();
        var mainVM = _serviceProvider.GetRequiredService<MainViewModel>();

        window.DataContext = mainVM;

        _previousWindow = _currentWindow;
        _currentWindow = window;
        window.Show();
    }

    public void ShowRegister()
    {
        var window = _serviceProvider.GetRequiredService<RegisterView>();

        var registerVM = _serviceProvider.GetRequiredService<RegisterViewModel>();
        window.DataContext = registerVM;

        _previousWindow = _currentWindow;
        _currentWindow = window;
        window.Show();
    }

    public void ShowLogin()
    {
        var window = _serviceProvider.GetRequiredService<LoginView>();

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
        _previousWindow?.Close(); // Close previous window if exists
    }
}