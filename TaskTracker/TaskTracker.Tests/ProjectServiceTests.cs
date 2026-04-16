using Moq;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Models;

namespace TaskTracker.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _repositoryMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _service = new ProjectService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateProjectAsync_Throws_ProjectNameEmpty_Key()
    {
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateProjectAsync("", "desc", 1));

        Assert.Equal("ProjectNameEmpty", ex.Message);
    }

    [Fact]
    public async Task UpdateProjectAsync_Throws_ProjectNotFound_Key()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync((Project?)null);

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.UpdateProjectAsync(10, "Name", "Desc", 1));

        Assert.Equal("ProjectNotFound", ex.UserMessage);
    }

    [Fact]
    public async Task UpdateProjectAsync_Throws_AccessDenied_Key()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Project { Id = 1, UserId = 99, Name = "Test" });

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.UpdateProjectAsync(1, "Name", "Desc", 1));

        Assert.Equal("AccessDenied", ex.UserMessage);
    }

    [Fact]
    public async Task GetUserProjectsAsync_Returns_Only_User_Projects()
    {
        _repositoryMock.Setup(r => r.GetUserProjectsAsync(1))
            .ReturnsAsync(new List<Project>
            {
                new() { Id = 1, Name = "P1", UserId = 1, CreatedAt = DateTime.Now },
                new() { Id = 2, Name = "P2", UserId = 1, CreatedAt = DateTime.Now }
            });

        var result = await _service.GetUserProjectsAsync(1);

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(r => r.GetUserProjectsAsync(1), Times.Once);
    }
}