using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Contracts.Responses;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.API.Controllers;
/// <summary>
/// Controller for managing projects (CRUD).
/// All endpoints require authentication.
/// </summary>

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retrieves all projects belonging to the authenticated user.
    /// </summary>
    /// <returns>
    ///   - 200 OK with the list of projects.
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMyProjects()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Gets a single project by its ID.
    /// </summary>
    /// <param name="projectId">The ID of the project to retrieve.</param>
    /// <returns>
    ///   - 200 OK with the project if found.
    ///   - 404 Not Found if the project does not exist.
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [HttpGet("GetById/{projectId}")]
    public async Task<IActionResult> GetById(int projectId)
    {
        try
        {
            var project = await _projectService.GetByIdAsync(projectId);
            if (project == null)
                return NotFound(new ErrorResponse { TranslationKey = "ProjectNotFound" });

            return Ok(project);
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Creates a new project for the authenticated user.
    /// </summary>
    /// <param name="request">Project data (name and optional description).</param>
    /// <returns>
    ///   - 200 OK with the created project (ID and name).
    ///   - 400 Bad Request if the project name is empty or invalid.
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var project = await _projectService.CreateProjectAsync(
                request.Name,
                request.Description,
                userId);

            return Ok(new Project
            {
                Id = project.Id,
                Name = project.Name
            });
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="projectId">The ID of the project to update.</param>
    /// <param name="project">Updated project data (name and description).</param>
    /// <returns>
    ///   - 204 No Content on success.
    ///   - 401 Unauthorized if the user is not authenticated.
    ///   - 403 Forbidden if the user is not the owner.
    ///   - 404 Not Found if the project does not exist.
    /// </returns>
    [HttpPut("{projectId}")]
    public async Task<IActionResult> UpdateProject(int projectId, [FromBody] Project project)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            await _projectService.UpdateProjectAsync(projectId, project.Name, project.Description, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Deletes a project by ID.
    /// </summary>
    /// <param name="projectId">The ID of the project to delete.</param>
    /// <returns>
    ///   - 204 No Content on success.
    ///   - 401 Unauthorized if the user is not authenticated.
    ///   - 403 Forbidden if the user is not the owner.
    ///   - 404 Not Found if the project does not exist.
    /// </returns>
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            await _projectService.DeleteProjectAsync(projectId, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    private IActionResult ToErrorResponse(Exception ex)
    {
        return ex switch
        {
            AppException appEx when appEx.UserMessage == "AccessDenied"
                => StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse { TranslationKey = appEx.UserMessage }),
            AppException appEx when appEx.UserMessage == "ProjectNotFound"
                => NotFound(new ErrorResponse { TranslationKey = appEx.UserMessage }),
            DomainException domainEx
                => BadRequest(new ErrorResponse { TranslationKey = domainEx.Message }),
            AppException appEx
                => BadRequest(new ErrorResponse { TranslationKey = appEx.UserMessage }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { TranslationKey = "UnexpectedError" })
        };
    }
}