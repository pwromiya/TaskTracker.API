using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.UI.Services;
using TaskTracker.Application.ApiInterfaces;
using TaskTracker.Application.Services;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.UI.ViewModels;

// Manages tasks list for selected project (WorkSpaceView)
public class TasksViewModel : BaseViewModel
{
    public ITaskApiService TaskService { get; }  // Tasks main logic

    private readonly IMessageService _messageService; // User notifications
    private readonly ILanguageService _languageService; // Localization support
    private readonly IErrorHandler _errorHandler;

    public ObservableCollection<TaskDto> ProjectTasks { get; } = new();

    private TaskDto? _selectedTask;
    public TaskDto? SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            OnPropertyChanged();  // Notify UI of selection change
        }
    }

    private int? _selectedProjectId;
    public int? SelectedProjectId
    {
        get => _selectedProjectId;
        set
        {
            _selectedProjectId = value;
            OnPropertyChanged();
            _ = LoadTasksAsync(); // Auto-load tasks on project change
        }
    }
    public ICommand DeleteTaskCommand { get; }

    public TasksViewModel(
        ITaskApiService taskService,
        IMessageService messageService,
        ILanguageService languageService,
        IErrorHandler errorHandler)
    {
        TaskService = taskService;
        _messageService = messageService;
        _languageService = languageService;
        _errorHandler = errorHandler;

        // Bind delete command with task parameter
        DeleteTaskCommand = new RelayCommand(async parameter =>
        {
            if (parameter is TaskDto task)
                await DeleteTaskAsync(task);
        });

    }

    // Load tasks for project
    public async Task LoadTasksAsync()
    {
        try
        {
            ProjectTasks.Clear(); // Clear current list

            if (_selectedProjectId == null)
                return;

            var tasks = await TaskService.GetByProjectIdAsync(_selectedProjectId.Value);

            foreach (var t in tasks)
                ProjectTasks.Add(t); // Populate observable collection
        }
        catch (Exception ex)
        {
            _errorHandler.Handle(ex);
        }
    }

    private async Task DeleteTaskAsync(TaskDto task)
    {
        try
        {
            if (task == null)
                return;

            // Delete confirmation
            if (!_messageService.Confirm(
            string.Format(_languageService.GetString("DeleteTaskConfirm"),task.Title),
            _languageService.GetString("Del")))
                return;

            await TaskService.DeleteTaskAsync(task.Id);

            ProjectTasks.Remove(task); // Remove task locally without full reload
        }
        catch (Exception ex)
        {
            _errorHandler.Handle(ex);
        }
    }

    public void Clear()
    {
        ProjectTasks.Clear();
    }
}