using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.Contracts.Requests;
using TaskTracker.Domain.Models;
using TaskTracker.UI.Api;
using TaskTracker.UI.Services;

namespace TaskTracker.UI.ViewModels;

// Manages projects list and user actions on projects (WorkSpaceView)
public class ProjectsViewModel : BaseViewModel
{
    public IProjectApiService ProjectService { get; }  // Projects main logic
    private readonly IMessageService _messageService; // User notifications

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
        IMessageService messageService)
    {
        ProjectService = projectService;
        _messageService = messageService;

        AddProjectCommand = new RelayCommand(async _ => await AddProjectAsync());
        DeleteProjectCommand = new RelayCommand(async _ => await DeleteProjectAsync());
        ShowAddProjectPopupCommand = new RelayCommand(_ => IsAddProjectPopupOpen = true);

        _ = LoadProjectsAsync();  // Auto-load projects on start
    }

    public async Task LoadProjectsAsync()
    {
        Projects.Clear();
        var projects = await ProjectService.GetUserProjectsAsync();
        foreach (var p in projects)
            Projects.Add(p);
    }

    private async Task AddProjectAsync()
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

    // Delete selected project with confirmation
    private async Task DeleteProjectAsync()
    {
        if (SelectedProject == null)
            return;
        if (!_messageService.Confirm(
        string.Format(LocalizationManager.GetString("DeleteProjectConfirm"), SelectedProject.Name),
        LocalizationManager.GetString("Del")))
            return;

        await ProjectService.DeleteProjectAsync(SelectedProject.Id);
        Projects.Remove(SelectedProject); // Remove from UI
    }

    public void Clear()
    {
        Projects.Clear();
    }
}