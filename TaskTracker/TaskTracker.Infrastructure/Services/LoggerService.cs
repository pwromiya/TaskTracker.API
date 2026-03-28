using TaskTracker.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace TaskTracker.Infrastructure.Services;

// Service from the infrastructure layer for writing information to a file
public class LoggerService:ILoggerService
{
    private readonly ILogger _logger;
    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }
    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        _logger.LogError(ex, message, args);
    }

}
