using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Contracts.Requests;

namespace TaskTracker.API.Controllers;
/// <summary>
/// Controller for user profile operations.
/// All endpoints require authentication.
/// </summary>

[Authorize]
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Returns basic information about the currently authenticated user.
    /// </summary>
    /// <returns>
    ///   - 200 OK with an object containing the user's ID.
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        return Ok(new { Id = userId });
    }

    /// <summary>
    /// Changes the password for the currently authenticated user.
    /// </summary>
    /// <param name="request">Contains the new password.</param>
    /// <returns>
    ///   - 200 OK on success.
    ///   - 400 Bad Request if the new password is empty or invalid.
    ///   - 401 Unauthorized if the user is not authenticated.
    /// </returns>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _userService.ChangePasswordAsync(userId, request.NewPassword);

        return Ok();
    }
}