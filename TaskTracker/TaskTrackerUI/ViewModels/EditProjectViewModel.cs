using System.ComponentModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.UI.Api;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.UI.ViewModels;

// ViewModel for create/update project (EditProjectControl)
public class EditProjectViewModel : INotifyPropertyChanged
{
    private readonly IProjectApiService _projectService; // The main logic service of this model
    private readonly IErrorHandler _errorHandler;   // Handles application errors
    private readonly int _projectId;
    private readonly bool _isEditMode;  // True if editing existing project
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action? Saved; // Raised after successful save
    public event Action? Canceled; // Raised on cancel

    public EditProjectViewModel(
        IProjectApiService projectService,
        ProjectDto? project = null,
        IErrorHandler errorHandler = null
        )
    {
        _projectService = projectService;
        _errorHandler = errorHandler;

        if (project != null)
        {
            _isEditMode = true;
            _projectId = project.Id;
            Name = project.Name;
            Description = project.Description;
        }

        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        CancelCommand = new RelayCommand(_ => Canceled?.Invoke());
        
    }

    // Saves project (create or update)
    private async Task SaveAsync()
    {
        if (_isEditMode)
        {
            try
            {
                await _projectService.UpdateProjectAsync(_projectId, Name, Description);
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex);
            }
        }
        else
        {
            try
            {
                await _projectService.CreateProjectAsync(Name, Description);
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex);
            }
        }

        Saved?.Invoke();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}