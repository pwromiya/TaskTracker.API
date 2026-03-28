using TaskTracker.Domain.Models;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.Application.Interfaces;

// Interface for CRUD with Project
public interface IProjectService
{
    Task<Project> CreateProjectAsync(string name, string? description, int userId);
    Task<List<ProjectDto>> GetUserProjectsAsync(int userId);
    Task<Project?> GetByIdAsync(int projectId);
    Task UpdateProjectAsync(int projectId,string Name,string Description, int userId);
    Task DeleteProjectAsync(int projectId, int userId);

}