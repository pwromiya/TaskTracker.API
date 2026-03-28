using Moq;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Models;
using TaskTracker.Domain.Common;

// Testing UserService from Application layer
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;

    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _service = new UserService(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    // Register tests

    [Fact]
    public async Task RegisterAsync_Should_Create_User_When_Login_Not_Exists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ExistsAsync("test")).ReturnsAsync(false);

        // Act
        var result = await _service.RegisterAsync("test", "111");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test", result.Login);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        _loggerMock.Verify(
            l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Login_Empty()
    {
        await Assert.ThrowsAsync<AppException>(() =>
            _service.RegisterAsync("", "1234"));
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Password_Empty()
    {
        await Assert.ThrowsAsync<AppException>(() =>
            _service.RegisterAsync("test", ""));
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_User_Exists()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ExistsAsync("test"))
            .ReturnsAsync(true);

        // Act
        await Assert.ThrowsAsync<AppException>(() =>
            _service.RegisterAsync("test", "1234"));

        // Assert
        _loggerMock.Verify(
            l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
    }

    // Login tests

    [Fact]
    public async Task LoginAsync_Should_Return_User_When_Correct()
    {
        // Arrange
        var salt = Guid.NewGuid().ToString();

        var user = new User
        {
            Id = 1,
            Login = "test",
            PasswordSalt = salt,
            PasswordHash = ComputeHash("1234", salt)
        };

        _repositoryMock
            .Setup(r => r.GetByLoginAsync("test"))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync("test", "1234");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test", result.Login);

        _loggerMock.Verify(
            l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Null_When_Wrong_Password()
    {
        // Arrange
        var user = new User
        {
            Login = "test",
            PasswordSalt = "salt",
            PasswordHash = "wronghash"
        };

        _repositoryMock
            .Setup(r => r.GetByLoginAsync("test"))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync("test", "1234");

        // Assert
        Assert.Null(result);

        _loggerMock.Verify(
            l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Null_When_Login_Empty()
    {
        var result = await _service.LoginAsync("", "1234");

        Assert.Null(result);
    }

    // Pasword tests
    [Fact]
    public async Task ChangePasswordAsync_Should_Update_Password()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            PasswordSalt = "old",
            PasswordHash = "oldhash"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        await _service.ChangePasswordAsync(1, "newpass");

        // Assert
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        Assert.NotNull(user.PasswordHash);
        Assert.NotEqual("oldhash", user.PasswordHash);
        Assert.NotEqual("old", user.PasswordSalt);

        _loggerMock.Verify(
            l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Throw_When_Empty()
    {
        await Assert.ThrowsAsync<AppException>(() =>
            _service.ChangePasswordAsync(1, ""));
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((User?)null);

        // Assert
        await Assert.ThrowsAsync<AppException>(() =>
            _service.ChangePasswordAsync(1, "newpass"));
    }

    private string ComputeHash(string password, string salt)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(
            System.Text.Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToBase64String(bytes);
    }
}