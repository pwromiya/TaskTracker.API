using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Domain.Models;

// Task status enumeration
public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Review = 2,
    Blocked = 3,
    Done = 4,
    Cancelled = 5
}

// Project task Domain model
public class ProjectTask : INotifyPropertyChanged
{
    private TaskStatus _status;
    private string _title = "";
    private string? _description;

    public int Id { get; set; }

    [NotMapped]
    public Action? StatusChangedCallback { get; set; }

    public TaskStatus Status
    {
        get => _status;
        set
        {
            if (_status == value) return;

            _status = value;
            OnPropertyChanged(nameof(Status));

            StatusChangedCallback?.Invoke();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;

            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (_description == value) return;

            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public int ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProjectTask()
    {
        CreatedAt = DateTime.Now;

        _status = TaskStatus.Todo;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public Project Project { get; set; } = null!;
}