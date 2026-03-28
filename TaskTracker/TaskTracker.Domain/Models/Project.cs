using System.ComponentModel;

namespace TaskTracker.Domain.Models;

// Project Domain model
public class Project : INotifyPropertyChanged
{
    private string? _description;

    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }
    public List<ProjectTask> Tasks { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}