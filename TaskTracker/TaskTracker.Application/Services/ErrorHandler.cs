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

        string messageKey = ex switch
        {
            AppException appEx => appEx.UserMessage,
            DomainException domainEx => domainEx.Message,

            HttpRequestException httpEx => "ErrorConnectionRefused",

            ArgumentException argEx => argEx.Message,
            _ => "UnexpectedError"
        };

        _messageService.ShowError(_languageService.GetString(messageKey));
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