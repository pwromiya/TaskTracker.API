using TaskTracker.Contracts.Requests;

namespace TaskTracker.UI.Api;

/// <summary>
/// A service providing project-related API operations.
/// </summary>
public interface IProjectApiService
{
    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="name">Project name.</param>
    /// <param name="description">Project description (optional).</param>
    /// <returns>The created project DTO.</returns>
    Task<ProjectDto> CreateProjectAsync(string name, string description);
    
    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <returns>List of all projects.</returns>
    Task<List<ProjectDto>> GetProjectsAsync();

    /// <summary>
    /// Retrieves projects belonging to the authenticated user.
    /// </summary>
    /// <returns>List of user's projects.</returns>
    Task<List<ProjectDto>> GetUserProjectsAsync();

    /// <summary>
    /// Deletes a project by ID.
    /// </summary>
    /// <param name="id">Project ID.</param>
    Task DeleteProjectAsync(int id);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="projectId">ID of the project to update.</param>
    /// <param name="name">New name.</param>
    /// <param name="description">New description (optional).</param>
    Task UpdateProjectAsync(int projectId, string name, string? description);
}
