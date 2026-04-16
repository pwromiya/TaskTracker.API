using Moq;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Models;

namespace TaskTracker.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_Throws_When_Login_Empty()
    {
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.RegisterAsync("", "1234"));

        Assert.Equal("EnterLogin", ex.UserMessage);
    }

    [Fact]
    public async Task RegisterAsync_Throws_When_Password_Empty()
    {
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.RegisterAsync("test", ""));

        Assert.Equal("PasswordEmpty", ex.UserMessage);
    }

    [Fact]
    public async Task RegisterAsync_Throws_When_User_Exists()
    {
        _repositoryMock
            .Setup(r => r.ExistsAsync("test"))
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.RegisterAsync("test", "1234"));

        Assert.Equal("UserAlreadyExists", ex.UserMessage);
    }

    [Fact]
    public async Task LoginAsync_Returns_Null_When_Login_Empty()
    {
        var result = await _service.LoginAsync("", "1234");
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_Returns_Null_When_Password_Empty()
    {
        var result = await _service.LoginAsync("test", "");
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_Returns_Null_When_Wrong_Password()
    {
        _repositoryMock
            .Setup(r => r.GetByLoginAsync("test"))
            .ReturnsAsync(new User
            {
                Login = "test",
                PasswordSalt = "salt",
                PasswordHash = "wronghash"
            });

        var result = await _service.LoginAsync("test", "1234");
        Assert.Null(result);
    }
}