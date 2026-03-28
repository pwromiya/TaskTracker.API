namespace TaskTracker.Application.Interfaces;
// Service interface for error messages
public interface IErrorHandler
{
    void Handle(Exception ex);
}
