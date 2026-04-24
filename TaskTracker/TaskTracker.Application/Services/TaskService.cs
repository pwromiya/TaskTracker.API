using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Models;
using TaskTracker.Domain.Common;

namespace TaskTracker.Application.Services;

// Service for TasksViewModel (CRUD with Task)
public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;   // Abstraction for Task data access (Infrastructure layer)
    private readonly IProjectRepository _projectRepository; // Abstraction only for explicit project binding in CreateTaskAsync
    private readonly ILoggerService _logger;

    public TaskService(
        ITaskRepository repository,
        IProjectRepository projectRepository,
        ILoggerService logger
        )
    {
        _repository = repository;
        _projectRepository = projectRepository;
        _logger = logger;
    }

    // Create
    public async Task<ProjectTask> CreateTaskAsync(
        string title,
        string? description,
        int projectId,
        int status)
    {
        // Valid task name 
        if (string.IsNullOrWhiteSpace(title))
            throw new AppException("TaskNameEmpty");

        // If project exists
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new AppException("ProjectNotFound");
        if (!Enum.IsDefined(typeof(Domain.Models.TaskStatus), status))
            throw new AppException("InvalidTaskStatus");

        var task = new ProjectTask
        {
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Status = (Domain.Models.TaskStatus)status,
            ProjectId = projectId,
            Project = project,  // Explicitly link the project
            CreatedAt = DateTime.Now
        };

        // Save to database
        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Adding new task with id: {taskId} for project with id: {taskProjectId} for user with id: {taskProjectUserId}",
            task.Id,
            task.ProjectId,
            task.Project.UserId);  // Project is not null

        return task;
    }

    // Read
    public async Task<List<ProjectTask>> GetByProjectIdForUserAsync(int projectId, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
            throw new AppException("ProjectNotFound");
        if (project.UserId != userId)
            throw new AppException("AccessDenied");

        return await _repository.GetByProjectIdAsync(projectId);
    }

    public async Task<ProjectTask?> GetByIdAsync(int taskId)
    {
        return await _repository.GetByIdAsync(taskId);
    }

    // Update
    public async Task UpdateTaskAsync(
        int taskId,
        string? title,
        string? description,
        Domain.Models.TaskStatus? status,
        int userId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null)
            throw new AppException("TaskNotFound");

        // Only owner can modify
        if (!await CanUserModifyTaskAsync(taskId, userId))
            throw new AppException("AccessDenied");

        // Update Title
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new AppException("TaskNameEmpty");
        }
        else
        {
            var newTitle = title.Trim();
            if (task.Title != newTitle)
            {
                _logger.LogInformation(
                    "Task updated: Id={TaskId}, ProjectId={ProjectId}, UserId={UserId}, Title: '{Old}' -> '{New}'",
                    task.Id, task.ProjectId, task.Project.UserId, task.Title, newTitle);
                task.Title = newTitle;
            }
        }

        // Update Description
        if (description != null)
        {
            var newDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            if (task.Description != newDescription)
            {
                _logger.LogInformation(
                    "Task updated: Id={TaskId}, ProjectId={ProjectId}, UserId={UserId}, Description changed",
                    task.Id, task.ProjectId, task.Project.UserId);
                task.Description = newDescription;
            }
        }

        // Update Status
        if (status.HasValue && task.Status != status.Value)
        {
            _logger.LogInformation(
                "Task updated: Id={TaskId}, ProjectId={ProjectId}, UserId={UserId}, Status: {Old} -> {New}",
                task.Id, task.ProjectId, task.Project.UserId, task.Status, status.Value);
            task.Status = status.Value;
        }

        await _repository.SaveChangesAsync();
    }

    // Delete
    public async Task DeleteTaskAsync(int taskId, int userId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null)
            throw new AppException("TaskNotFound");

        // Only owner can delete
        if (!await CanUserModifyTaskAsync(taskId, userId))
            throw new AppException("AccessDenied");

        _repository.Remove(task);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Deleting new task with id: {taskId} for project with id: {taskProjectId} by user with id: {taskProjectUserId}", task.Id, task.ProjectId, task.Project.UserId);
    }

    
    public async Task<bool> CanUserModifyTaskAsync(int taskId, int userId)
    {
        return await _repository.CanUserModifyTaskAsync(taskId, userId);
    }
}
