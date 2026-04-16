using TaskTracker.Contracts.Responses;

namespace TaskTracker.Application.ApiInterfaces;

/// <summary>
/// A service providing authentication operations.
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="login">User login.</param>
    /// <param name="password">User password.</param>
    /// <returns>Registration response with user data and token, or null if registration failed.</returns>
    Task<RegisterResponse?> Register(string login, string password);

    /// <summary>
    /// Logs in a user and returns JWT token.
    /// </summary>
    /// <param name="login">User login.</param>
    /// <param name="password">User password.</param>
    /// <returns>Login response with token and user data, or null if credentials invalid.</returns>
    /// <remarks>On success, stores token in TokenStorage and sets Authorization header.</remarks>
    Task<LoginResponse?> Login(string login, string password);

    /// <summary>
    /// Clears stored authentication token.
    /// </summary>
    public void ClearToken();
}
