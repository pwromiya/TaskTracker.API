namespace TaskTracker.Application.Interfaces;

//An abstraction for logging that connects the corresponding service(Infrastructure) to the Application layer
public interface ILoggerService
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception ex, string message, params object[] args);
}
