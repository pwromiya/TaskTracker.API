using Microsoft.AspNetCore.Mvc;
using TaskTracker.Contracts.Requests;
using TaskTracker.Contracts.Responses;
using TaskTracker.Domain.Common;

namespace TaskTracker.API.Controllers;
/// <summary>
/// Controller for user authentication (registration and login).
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase    
{
    private readonly IUserService _userService;
    private readonly JwtService _jwtService;

    public AuthController(IUserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">Registration request containing login and password.</param>
    /// <returns>
    ///   - 200 OK with the created user's ID, login, and JWT token.
    ///   - 400 Bad Request if validation fails or the login is already taken.
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = await _userService.RegisterAsync(request.Login, request.Password);
            var token = _jwtService.GenerateToken(user);
            return Ok(new RegisterResponse
            {
                Id = user.Id,
                Login = user.Login,
                Token = token
            });
        }
        catch (AppException ex)
        {
            return BadRequest(new { message = ex.UserMessage });
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">Login request containing login and password.</param>
    /// <returns>
    ///   - 200 OK with user data and JWT token if credentials are valid.
    ///   - 401 Unauthorized if the login or password is incorrect.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.LoginAsync(request.Login, request.Password);

        if (user == null)
            return Unauthorized();

        var token = _jwtService.GenerateToken(user);
        return Ok(new LoginResponse
        {
            Id = user.Id,
            Login = user.Login,
            Token = token
        });
    }
}