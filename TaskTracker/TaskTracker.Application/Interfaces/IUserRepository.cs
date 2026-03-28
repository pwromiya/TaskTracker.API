using TaskTracker.Domain.Models;

// Abstraction for connecting the Application layer with Infrastructure
public interface IUserRepository
{
    Task<bool> ExistsAsync(string login);
    Task AddAsync(User user);
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetByIdAsync(int id);
    Task SaveChangesAsync();
}