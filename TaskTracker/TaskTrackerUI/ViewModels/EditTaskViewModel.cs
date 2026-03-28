using System.ComponentModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.Contracts.Requests;
using TaskTracker.UI.Api;

namespace TaskTracker.UI.ViewModels;

// ViewModel for create/update task (EditTaskControl)
public class EditTaskViewModel : INotifyPropertyChanged
{
    private readonly ITaskApiService _taskApiService;  // The main logic service of this model
    private readonly IErrorHandler _errorHandler;   // Handles application errors
    private readonly int _taskId;
    private readonly int _projectId;
    private readonly bool _isEditMode;  // True if editing existing task

    public Array Statuses => Enum.GetValues(typeof(Domain.Models.TaskStatus));
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public Domain.Models.TaskStatus Status { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action? Saved; // Raised after successful save
    public event Action? Canceled; // Raised on cancel

    public EditTaskViewModel(
        ITaskApiService taskService,
        int projectId,
        TaskDto? task = null,
        IErrorHandler errorHandler = null)
    {
        _taskApiService = taskService;
        _projectId = projectId;

        if (task != null)
        {
            _isEditMode = true;
            _taskId = task.Id;
            Title = task.Title;
            Description = task.Description;
            Status = (Domain.Models.TaskStatus)task.Status;
        }

        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        CancelCommand = new RelayCommand(_ => Canceled?.Invoke());
        _errorHandler = errorHandler;
    }

    // Saves task (create or update)
    private async Task SaveAsync()
    {
        if (_isEditMode)
        {
            await _taskApiService.UpdateTaskAsync(
                _taskId,
                Title,
                Description,
                (int)Status);
        }
        else
        {
            try
            {
                await _taskApiService.CreateTaskAsync(
                Title,
                Description,
                _projectId,
                (int)Status);
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