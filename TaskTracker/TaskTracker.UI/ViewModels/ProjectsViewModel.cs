using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.Contracts.Requests;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.UI.Services;

namespace TaskTracker.UI.ViewModels;

// Manages projects list and user actions on projects (WorkSpaceView)
public class ProjectsViewModel : BaseViewModel
{
    public IProjectApiService ProjectService { get; }  // Projects main logic
    private readonly IMessageService _messageService; // User notifications
    private readonly ILanguageService _languageService; // Localization support
    private readonly IErrorHandler _erorHandler; // Centralized error handling

    // Controls Add Project popup (Editing user control) visibility
    private bool _isAddProjectPopupOpen;
    public bool IsAddProjectPopupOpen
    {
        get => _isAddProjectPopupOpen;
        set
        {
            _isAddProjectPopupOpen = value;
            OnPropertyChanged();  // Update UI popup state
        }
    }

    private ProjectDto? _selectedProject;
    public ProjectDto? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            OnPropertyChanged();  // Notify UI of selection change
        }
    }

    // New project form fields
    public string NewProjectName { get; set; } = "";
    public string NewProjectDescription { get; set; } = "";

    // Collection of user's projects (UI binding)
    public ObservableCollection<ProjectDto> Projects { get; } = new();

    public ICommand AddProjectCommand { get; }
    public ICommand DeleteProjectCommand { get; }
    public ICommand ShowAddProjectPopupCommand { get; }

    public ProjectsViewModel(
        IProjectApiService projectService,
        IMessageService messageService,
        ILanguageService languageService,
        IErrorHandler errorHandler)
    {
        ProjectService = projectService;
        _messageService = messageService;
        _languageService = languageService;
        _erorHandler = errorHandler;
        AddProjectCommand = new RelayCommand(async _ => await AddProjectAsync());
        DeleteProjectCommand = new RelayCommand(async _ => await DeleteProjectAsync());
        ShowAddProjectPopupCommand = new RelayCommand(_ => IsAddProjectPopupOpen = true);

        _ = LoadProjectsAsync();  // Auto-load projects on start

    }

    public async Task LoadProjectsAsync()
    {
        try
        {
            Projects.Clear();
            var projects = await ProjectService.GetUserProjectsAsync();
            foreach (var p in projects)
                Projects.Add(p);
        }
        catch (Exception ex)
        {
            _erorHandler.Handle(ex);
        }
    }

    private async Task AddProjectAsync()
    {
        try
        {
            var project = await ProjectService.CreateProjectAsync(
                NewProjectName,
                NewProjectDescription);

            // Add to UI collection
            Projects.Add(project);

            // Clear form fields
            NewProjectName = "";
            NewProjectDescription = "";
            OnPropertyChanged(nameof(NewProjectName));
            OnPropertyChanged(nameof(NewProjectDescription));
            IsAddProjectPopupOpen = false;
        }
        catch (Exception ex)
        {
            _erorHandler.Handle(ex);
        }
    }

    // Delete selected project with confirmation
    private async Task DeleteProjectAsync()
    {
        try
        {
            if (SelectedProject == null)
                return;
            if (!_messageService.Confirm(
            string.Format(_languageService.GetString("DeleteProjectConfirm"), SelectedProject.Name),
            _languageService.GetString("Del")))
                return;

            await ProjectService.DeleteProjectAsync(SelectedProject.Id);
            Projects.Remove(SelectedProject); // Remove from UI
        }
        catch (Exception ex)
        {
            _erorHandler.Handle(ex);
        }
    }

    public void Clear()
    {
        Projects.Clear();
    }
}