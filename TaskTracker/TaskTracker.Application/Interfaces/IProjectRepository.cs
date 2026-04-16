using TaskTracker.Domain.Models;

namespace TaskTracker.Application.Interfaces;

// Abstraction for connecting the Application layer with Infrastructure (CRUD for Project model)
public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task<Project?> GetByIdAsync(int id);
    Task<List<Project>> GetUserProjectsAsync(int userId);
    Task SaveChangesAsync();
    void Remove(Project project);
    Task<bool> ExistsAsync(string name, int userId);

}
