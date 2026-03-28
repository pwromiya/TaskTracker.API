using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Models;

// Service for LoginViewModel, RegisterViewModel
public class UserService : IUserService
{
    private readonly IUserRepository _repository;   // Abstraction for User data access (Infrastructure layer)
    private readonly ILoggerService _logger;

    public UserService(IUserRepository repository, ILoggerService logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<User> RegisterAsync(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new AppException("EnterLogin");

        // Check for existing user
        if (await _repository.ExistsAsync(login))
        {
            _logger.LogWarning("Attempt to register existing login: {Login}", login);
            throw new AppException("UserAlreadyExists");
        }
        // Create user with hashed password
        var salt = Guid.NewGuid().ToString();
        var user = new User
        {
            Login = login.Trim(),
            PasswordSalt = salt,
            PasswordHash = HashPassword(password, salt)
        };

        //Save to database
        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {Login}, Id={Id}", user.Login, user.Id);

        return user;
    }

    public async Task<User?> LoginAsync(string login, string password)
    {
        // Find user by login
        var user = await _repository.GetByLoginAsync(login);

        // User exists and passworm matches
        if (user == null || HashPassword(password, user.PasswordSalt) != user.PasswordHash)
        {
            _logger.LogWarning("Invalid login attempt for user {Login}", login);
            return null;
        }

        _logger.LogInformation("User logged in successfully: {Login}, Id={Id}", user.Login, user.Id);
        return user;
    }

    // Hash password with salt using SHA256
    private string HashPassword(string password, string salt)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToBase64String(bytes);
    }

    public async Task ChangePasswordAsync(int userId, string newPassword)
    {
        // Validate new password
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new AppException("PasswordEmpty");

        // Find user by id
        var user = await _repository.GetByIdAsync(userId);

        if (user == null)
            throw new AppException("UserNotFound");

        var salt = Guid.NewGuid().ToString();
        var hash = HashPassword(newPassword, salt);

        // Update salt and hash
        user.PasswordSalt = salt;
        user.PasswordHash = hash;

        await _repository.SaveChangesAsync();
        _logger.LogInformation("Password on account with id: {UserId} successfully changed", user.Id);
    }
}