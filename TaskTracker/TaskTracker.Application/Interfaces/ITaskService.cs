using TaskTracker.Domain.Models;

namespace TaskTracker.Application.Interfaces;

// Interface for CRUD with Task
public interface ITaskService
{
    Task<ProjectTask> CreateTaskAsync(string title, string? description, int projectId, int status);
    Task<List<ProjectTask>> GetByProjectIdAsync(int projectId);
    Task<ProjectTask?> GetByIdAsync(int taskId);
    Task UpdateTaskAsync(int taskId, string? title, string? description, Domain.Models.TaskStatus? status, int userId);
    Task DeleteTaskAsync(int taskId, int userId);

    // Business logic
    Task<bool> CanUserModifyTaskAsync(int taskId, int userId);

}