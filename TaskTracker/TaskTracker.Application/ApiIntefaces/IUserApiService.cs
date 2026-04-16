namespace TaskTracker.Application.ApiInterfaces;
/// <summary>
/// A service providing API functions related to users.
/// </summary>
public interface IUserApiService
{
    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    /// <returns>User ID.</returns>
    /// <exception cref="HttpRequestException">Thrown if the request fails.</exception>
    Task<int> GetCurrentUserIdAsync();

    /// <summary>
    /// Changes the password for the currently authenticated user.
    /// </summary>
    /// <param name="newPassword">New password.</param>
    /// <exception cref="HttpRequestException">Thrown if the request fails.</exception>
    Task ChangePasswordAsync(string newPassword);
}