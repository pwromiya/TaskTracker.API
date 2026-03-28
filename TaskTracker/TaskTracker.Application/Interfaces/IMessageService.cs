namespace TaskTracker.Application.Interfaces;

// Service for managing User-friendly notifications
public interface IMessageService
{
    public bool Confirm(string message, string title);
    public void ShowError(string message);
    public void ShowInformation(string message);
    public void ShowWarning(string message);
}

