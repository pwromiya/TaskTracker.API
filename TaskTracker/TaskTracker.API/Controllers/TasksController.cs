using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Application.Interfaces;
using TaskTracker.Contracts.Requests;
using TaskTracker.Contracts.Responses;
using TaskTracker.Domain.Common;

namespace TaskTracker.API.Controllers;
/// <summary>
/// Controller for managing tasks (CRUD).
/// All endpoints require authentication.
/// </summary>

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Retrieves all tasks belonging to a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>
    ///   - 200 OK with a list of tasks (TaskDto).
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var tasks = await _taskService.GetByProjectIdForUserAsync(projectId, userId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Gets a single task by its ID.
    /// </summary>
    /// <param name="taskId">The ID of the task to retrieve.</param>
    /// <returns>
    ///   - 200 OK with the task (TaskDto).
    ///   - 404 Not Found if the task does not exist.
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [HttpGet("GetById/{taskId}")]
    public async Task<IActionResult> GetById(int taskId)
    {
        try
        {
            var task = await _taskService.GetByIdAsync(taskId);
            if (task == null)
                return NotFound(new ErrorResponse { TranslationKey = "TaskNotFound" });

            return Ok(task);
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">Task data (title, description, project ID, status).</param>
    /// <returns>
    ///   - 201 Created with the created task (TaskDto) and a Location header.
    ///   - 400 Bad Request if validation fails (e.g., empty title).
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        try
        {
            var created = await _taskService.CreateTaskAsync(
                request.Title,
                request.Description,
                request.ProjectId,
                request.Status
            );

            var taskDto = new TaskDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                Status = (int)created.Status,
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { taskId = created.Id }, taskDto);
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="taskId">The ID of the task to update.</param>
    /// <param name="task">Updated task data (all fields).</param>
    /// <returns>
    ///   - 204 No Content on success.
    ///   - 401 Unauthorized if the user is not authenticated.
    ///   - 403 Forbidden if the user is not the owner of the task's project.
    ///   - 404 Not Found if the task does not exist.
    /// </returns>
    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(int taskId, [FromBody] TaskDto task)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            if (!Enum.IsDefined(typeof(Domain.Models.TaskStatus), task.Status))
                return BadRequest(new ErrorResponse { TranslationKey = "InvalidTaskStatus" });

            var userId = int.Parse(userIdClaim.Value);
            var domainStatus = (Domain.Models.TaskStatus)task.Status;
            await _taskService.UpdateTaskAsync(taskId, task.Title, task.Description, domainStatus, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="taskId">The ID of the task to delete.</param>
    /// <returns>
    ///   - 204 No Content on success.
    ///   - 401 Unauthorized if the user is not authenticated.
    ///   - 403 Forbidden if the user is not the owner of the task's project.
    ///   - 404 Not Found if the task does not exist.
    /// </returns>
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            await _taskService.DeleteTaskAsync(taskId, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return ToErrorResponse(ex);
        }
    }

    /// <summary>
    /// Checks whether a user has permission to modify a specific task.
    /// This endpoint is used for UI decisions (e.g., enabling edit/delete buttons).
    /// </summary>
    /// <param name="taskId">The ID of the task.</param>
    /// <param name="userId">The ID of the user to check.</param>
    /// <returns>
    ///   - 200 OK with a boolean value (true if user can modify, false otherwise).
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [HttpGet("CanModify/{taskId}/{userId}")]
    public async Task<IActionResult> CanUserModify(int taskId, int userId)
    {
        try
        {
            var canModify = await _taskService.CanUserModifyTaskAsync(taskId, userId);
            return Ok(canModify);
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
            AppException appEx when appEx.UserMessage == "TaskNotFound" || appEx.UserMessage == "ProjectNotFound"
                => NotFound(new ErrorResponse { TranslationKey = appEx.UserMessage }),
            AppException appEx
                => BadRequest(new ErrorResponse { TranslationKey = appEx.UserMessage }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { TranslationKey = "UnexpectedError" })
        };
    }
}