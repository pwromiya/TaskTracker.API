using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.UI.Services;
using TaskTracker.UI.ViewModels;
using TaskTracker.UI.Views;
using TaskTracker.UI.Views.UserControls;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers;
using Serilog.Events;
using System.IO;
using System.Net.Http;
using TaskTracker.Infrastructure.Data;
using TaskTracker.Infrastructure.Repositories;
using TaskTracker.Infrastructure.Services;
using TaskTracker.Infrastructure.Api;
using TaskTracker.Application.ApiInterfaces;

namespace TaskTracker.Startup;

// Main application class with DI configuration
public partial class App : System.Windows.Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        this.ShutdownMode = ShutdownMode.OnLastWindowClose;

        ConfigureSerilog();

        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


        DispatcherUnhandledException += (s, e) =>
        {
            var handler = ServiceProvider.GetRequiredService<IErrorHandler>();
            handler.Handle(e.Exception);
            e.Handled = true;
        };
        
        var services = new ServiceCollection();

        var apiBaseUrl = configuration["ApiSettings:BaseUrl"];

        services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        services.AddHttpClient<IProjectApiService, ProjectApiService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        services.AddHttpClient<ITaskApiService, TaskApiService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        services.AddHttpClient<IUserApiService, UserApiService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ProjectsViewModel>();
        services.AddTransient<TasksViewModel>();
        services.AddSingleton<LanguageViewModel>();

        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<RegisterView>();
        services.AddTransient<MainView>();

        // UI Services
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ILanguageService, LanguageService>();
        services.AddSingleton<TokenStorage>();

        // Infrastructure Services
        services.AddSingleton<ILoggerService, LoggerService>();

        services.AddSingleton<IErrorHandler, ErrorHandler>();

        services.AddLogging(builder =>
        {
            builder.ClearProviders(); // Remove standard providers
            builder.AddSerilog(); // Add Serilog as a provider
            builder.SetMinimumLevel(LogLevel.Debug);

            builder.AddFilter("Microsoft", LogLevel.Warning); // Removing unnecessary logs from Microsoft
            builder.AddFilter("System", LogLevel.Warning);
        });

        ServiceProvider = services.BuildServiceProvider();

        // Logging a successful launch
        var logger = ServiceProvider.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Application launched successfully");

        Current.Resources.Add("LangVM",
            ServiceProvider.GetRequiredService<LanguageViewModel>());

        var windowService = ServiceProvider.GetRequiredService<IWindowService>();
        windowService.ShowRegister();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void ConfigureSerilog()
    {
        var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        Directory.CreateDirectory(logDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine(logDirectory, "app-log-.txt"),
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 20 * 1024 * 1024,
                retainedFileCountLimit: 31,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        // Logging the start of work
        Log.Information("Launching the application");
    }
}