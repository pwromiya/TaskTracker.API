using System.Windows;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.UI.Services;

// Service for displaying messages to the user (MVVM-friendly)
public class MessageService : IMessageService
{
    // Show a confirmation dialog with Yes/No buttons
    public bool Confirm(string message, string title)
    {
        return MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }


    // Show an error message to the user
    public void ShowError(string message)
    {
        MessageBox.Show(
            message,
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    // Show an informational message to the user
    public void ShowInformation(string message)
    {
        MessageBox.Show(
            message,
            "Info",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // Show a warning message to the user
    public void ShowWarning(string message)
    {
        MessageBox.Show(
            message,
            "Warning",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}