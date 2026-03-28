using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.Application.Services;

// Service for ProjectsViewModel (CRUD with Project)
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;    // Abstraction for Project data access (Infrastructure layer)
    private readonly ILoggerService _logger;

    public ProjectService(
        IProjectRepository repository,
        ILoggerService logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    // Create
    public async Task<Project> CreateProjectAsync(string name, string? description, int userId)
    {
        // Valid project name 
        if (string.IsNullOrWhiteSpace(name))
            throw new AppException("ProjectNameEmpty");

        var project = new Project
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            UserId = userId,    // Assign to current user
            CreatedAt = DateTime.Now
        };

        await _repository.AddAsync(project);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Adding new project with id: {projectId} for user with id: {projectUserId}", project.Id, project.UserId);
        return project;
    }

    // Read
    public async Task<List<ProjectDto>> GetUserProjectsAsync(int userId)
    {
        var projects = await _repository.GetUserProjectsAsync(userId);

        return projects.Select(p => new ProjectDto  // Mapping Project to ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    public async Task<Project?> GetByIdAsync(int projectId)
    {
        return await _repository.GetByIdAsync(projectId);
    }

    // Update
    public async Task UpdateProjectAsync(
        int projectId,
        string? name,
        string? description,
        int userId)
    {
        var project = await _repository.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Only owner can modify
        if (project.UserId != userId)
            throw new UnauthorizedAccessException("No permission");

        // Update Name
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new AppException("ProjectNameEmpty");
        }
        else
        {
            var newName = name.Trim();
            if (project.Name != newName)
            {
                _logger.LogInformation(
                    "Project updated: Id={ProjectId}, UserId={UserId}, Name: '{Old}' -> '{New}'",
                    project.Id, project.UserId, project.Name, newName);
                project.Name = newName;
            }
        }

        // Update Description
        if (description != null)
        {
            var newDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            if (project.Description != newDescription)
            {
                _logger.LogInformation(
                    "Project updated: Id={ProjectId}, UserId={UserId}, Description changed",
                    project.Id, project.UserId);
                project.Description = newDescription;
            }
        }

        await _repository.SaveChangesAsync();
    }

    // Delete
    public async Task DeleteProjectAsync(int projectId, int userId)
    {
        var project = await _repository.GetByIdAsync(projectId);
        if (project == null)
            return;

        // Only owner can delete
        if (project.UserId != userId)
            throw new UnauthorizedAccessException("No permission");

        _repository.Remove(project);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Deleting project with id: {projectId} by user with id: {projectUserId}", project.Id, project.UserId);
    }
}
