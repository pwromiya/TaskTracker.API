using TaskTracker.Contracts.Requests;

namespace TaskTracker.Application.ApiInterfaces;

/// <summary>
/// A service providing task-related API operations.
/// </summary>
public interface ITaskApiService
{
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="title">Task title (required).</param>
    /// <param name="description">Optional description.</param>
    /// <param name="projectId">ID of the project this task belongs to.</param>
    /// <param name="status">Task status code (0=Todo, 1=InProgress, 2=Review, 3=Blocked, 4=Done, 5=Cancelled).</param>
    Task CreateTaskAsync(string title, string? description, int projectId, int status);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">Task ID.</param>
    /// <param name="title">New title.</param>
    /// <param name="description">New description (optional).</param>
    /// <param name="status">New status code.</param>
    Task UpdateTaskAsync(int id, string title, string? description, int status);

    /// <summary>
    /// Deletes a task by ID.
    /// </summary>
    /// <param name="id">Task ID.</param>
    Task DeleteTaskAsync(int id);

    /// <summary>
    /// Retrieves all tasks for a given project.
    /// </summary>
    /// <param name="projectId">Project ID.</param>
    /// <returns>List of tasks as DTOs.</returns>
    Task<List<TaskDto>> GetByProjectIdAsync(int projectId);
}
