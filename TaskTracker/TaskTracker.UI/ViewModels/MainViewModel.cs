using System.ComponentModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.Application.Services;
using TaskTracker.UI.Services;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.UI.ViewModels;

//MainViewModel orchestrates projects (ProjectsViewModel), tasks (TasksViewModel), and user interactions in the main window (MainView)

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IWindowService _windowService;
    private readonly IErrorHandler _errorHandler;
    private readonly IUserApiService _userApiService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService;
    private readonly TokenStorage _tokenStorage;

    private object? _currentDialog;
    public ProjectsViewModel ProjectsVM { get; }  // Sub-VM managing projects
    public TasksViewModel TasksVM { get; }  // Sub-VM managing projects

    public ICommand LogoutCommand { get; }
    public ICommand SaveUserProfileCommand { get; } // Change password command
    
    // For opening EditTaskControl/EditProjectControl
    public object? CurrentDialog
    {
        get => _currentDialog;
        set { _currentDialog = value; OnPropertyChanged(nameof(CurrentDialog)); }
    }
    public ICommand ShowAddProjectCommand { get; }
    public ICommand ShowEditProjectCommand { get; }

    public ICommand ShowAddTaskCommand { get; }
    public ICommand ShowEditTaskCommand { get; }
    public MainViewModel(
        ProjectsViewModel projectsVM,
        TasksViewModel tasksVM,
        IWindowService windowService,
        IErrorHandler errorHandler,
        IUserApiService userApiService,
        IMessageService messageService,
        IAuthApiService authApiService,
        TokenStorage tokenStorage
        ,
        ILanguageService languageService)
    {
        ProjectsVM = projectsVM;
        TasksVM = tasksVM;
        _windowService = windowService;
        _errorHandler = errorHandler;
        _userApiService = userApiService;
        _messageService = messageService;
        _tokenStorage = tokenStorage;
        _languageService = languageService;

        // Sync tasks with selected project
        ProjectsVM.PropertyChanged += OnProjectsPropertyChanged;

        LogoutCommand = new RelayCommand(_ => Logout());

        // Change password command
        SaveUserProfileCommand = new RelayCommand(async param =>
        {
            if (param is System.Windows.Controls.PasswordBox pb)
            {
                string newPassword = pb.Password;
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    _messageService.ShowWarning(_languageService.GetString("EnterPassword"));
                    return;
                }
                try
                {
                    await _userApiService.ChangePasswordAsync(newPassword);
                    _messageService.ShowInformation(_languageService.GetString("PasswordUpdated"));
                    pb.Clear();
                }
                catch (Exception ex)
                {
                    _errorHandler.Handle(ex);
                }
            }
        });

        // Commands to open project/task editor popups
        ShowAddProjectCommand = new RelayCommand(_ => ShowEditProject(null));
        ShowEditProjectCommand = new RelayCommand(p => ShowEditProject(p as ProjectDto));

        ShowAddTaskCommand = new RelayCommand(_ => ShowEditTask(null));
        ShowEditTaskCommand = new RelayCommand(t => ShowEditTask(t as TaskDto));

    }

    private void OnProjectsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProjectsViewModel.SelectedProject))
        {
            TasksVM.SelectedProjectId = ProjectsVM.SelectedProject?.Id; // Propagate selection change
        }
    }

    private void Logout()
    {
        _tokenStorage.Token = null;
        _windowService.ShowRegister();
        _windowService.ClosePrevious(); // Close main window
        TasksVM.Clear();
        ProjectsVM.Clear();
    }

    // Show project editor Usercontrol
    public void ShowEditProject(ProjectDto? project)
    {
        var vm = new EditProjectViewModel(ProjectsVM.ProjectService, project, _errorHandler);
        vm.Saved += async () =>
        {
            CurrentDialog = null;
            await ProjectsVM.LoadProjectsAsync();
        };
        vm.Canceled += () => CurrentDialog = null;

        CurrentDialog = vm;
    }

    // Show task editor popup
    public void ShowEditTask(TaskDto? task)
    {
        if (TasksVM.SelectedProjectId == null) return;

        var vm = new EditTaskViewModel(TasksVM.TaskService, TasksVM.SelectedProjectId.Value, task, _errorHandler);
        vm.Saved += async () =>
        {
            CurrentDialog = null;
            await TasksVM.LoadTasksAsync();
        };
        vm.Canceled += () => CurrentDialog = null;
        CurrentDialog = vm;
    }

    // Event for INotifyPropertyChanged; notifies UI when a property changes
    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to raise PropertyChanged event
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}