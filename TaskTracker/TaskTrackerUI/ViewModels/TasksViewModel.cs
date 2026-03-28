using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskTracker.Application.Interfaces;
using TaskTracker.Commands;
using TaskTracker.UI.Services;
using TaskTracker.UI.Api;
using TaskTracker.Application.Services;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.UI.ViewModels;

// Manages tasks list for selected project (WorkSpaceView)
public class TasksViewModel : BaseViewModel
{
    public ITaskApiService TaskService { get; }  // Tasks main logic

    private readonly IMessageService _messageService; // User notifications

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
        IMessageService messageService)
    {
        TaskService = taskService;
        _messageService = messageService;

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
        ProjectTasks.Clear(); // Clear current list

        if (_selectedProjectId == null)
            return;

        var tasks = await TaskService.GetByProjectIdAsync(_selectedProjectId.Value);

        foreach (var t in tasks)
            ProjectTasks.Add(t); // Populate observable collection
    }

    private async Task DeleteTaskAsync(TaskDto task)
    {
        if (task == null)
            return;

        // Delete confirmation
        if (!_messageService.Confirm(
        string.Format(LocalizationManager.GetString("DeleteTaskConfirm"),task.Title),
        LocalizationManager.GetString("Del")))
            return;

        await TaskService.DeleteTaskAsync(task.Id);

        ProjectTasks.Remove(task); // Remove task locally without full reload
    }

    public void Clear()
    {
        ProjectTasks.Clear();
    }
}