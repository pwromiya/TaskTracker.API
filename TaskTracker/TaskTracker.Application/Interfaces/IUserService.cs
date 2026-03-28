using TaskTracker.Domain.Models;

// Service interface for managing registration and login
public interface IUserService
{
    Task<User> RegisterAsync(string login, string password);
    Task<User?> LoginAsync(string login, string password);
    Task ChangePasswordAsync(int userId, string newPassword);
}