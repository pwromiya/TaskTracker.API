using TaskTracker.Domain.Models;

namespace TaskTracker.Application.Interfaces;

// Abstraction for connecting the Application layer with Infrastructure (CRUD for Task model)
public interface ITaskRepository
{
    Task AddAsync(ProjectTask task);
    Task<List<ProjectTask>> GetByProjectIdAsync(int projectId);
    Task<ProjectTask?> GetByIdAsync(int id);
    Task SaveChangesAsync();
    void Remove(ProjectTask task);
    // Business
    Task<bool> CanUserModifyTaskAsync(int taskId, int userId);
}
