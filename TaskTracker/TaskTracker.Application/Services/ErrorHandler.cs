using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;

namespace TaskTracker.Application.Services;

// Centralized error handling with logging and user notifications
public class ErrorHandler : IErrorHandler
{
    private readonly ILanguageService _languageService;
    private readonly IMessageService _messageService;
    private readonly ILoggerService _logger;

    public ErrorHandler(
        ILanguageService languageService,
        IMessageService messageService,
        ILoggerService logger)
    {
        _languageService = languageService;
        _messageService = messageService;
        _logger = logger;
    }

    public void Handle(Exception ex)
    {
        LogError(ex);

        // Determining message key
        string messageKey = ex switch
        {
            // Business exception
            AppException appEx => appEx.UserMessage,

            // Validation or argument error
            ArgumentException argEx => argEx.Message,

            // Unexpected error
            _ => "UnexpectedError"
        };

        // Get localized text
        string message = _languageService.GetString(messageKey);

        // Display error message to user via abstraction
        _messageService.ShowError(message);
    }

    private void LogError(Exception ex)
    {
        _logger.LogError(
            ex,
            "Unhandled exception: {Message}\n{StackTrace}",
            ex.Message,
            ex.StackTrace);
    }
}