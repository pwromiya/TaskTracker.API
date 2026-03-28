namespace TaskTracker.Domain.Common;

// Custom Business Exception
public class AppException : Exception
{
    public string UserMessage { get; }

    public AppException(string userMessage, Exception? inner = null)
        : base(userMessage, inner)
    {
        UserMessage = userMessage;
    }
}