using Xunit;
using Moq;
using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Application.Services;
using Wpf_Task4.Domain.Models;
using Wpf_Task4.Domain.Common;

// Testing TaskService from Application layer
public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ILoggerService> _loggerMock;

    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILoggerService>();

        _currentUserMock.Setup(c => c.CurrentUser)
            .Returns(new User { Id = 1 });

        _service = new TaskService(
            _repositoryMock.Object,
            _projectRepositoryMock.Object,
            _currentUserMock.Object,
            _loggerMock.Object);
    }

    // Create
    [Fact]
    public async Task CreateTaskAsync_Should_Create_Task()
    {
        // Arrange
        var project = new Project
        {
            Id = 5,
            UserId = 1,
            Name = "Test Project"
        };

        _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(project);

        // Act
        var result = await _service.CreateTaskAsync("Title", "Desc", 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Title", result.Title);
        Assert.Equal(5, result.ProjectId);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ProjectTask>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Throw_When_Title_Empty()
    {
        await Assert.ThrowsAsync<AppException>(() =>
            _service.CreateTaskAsync("", "Desc", 5));
    }

    // Update
    [Fact]
    public async Task UpdateTaskAsync_Should_Update_Title()
    {
        // Arrange
        var task = new ProjectTask
        {
            Id = 1,
            Title = "Old",
            ProjectId = 5,
            Project = new Project { UserId = 1 }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(task);

        _repositoryMock.Setup(r => r.CanUserModifyTaskAsync(1, 1))
            .ReturnsAsync(true);

        // Act
        await _service.UpdateTaskAsync(1, "New", null, null);

        // Assert
        Assert.Equal("New", task.Title);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Throw_When_NotOwner()
    {
        // Arrange
        var task = new ProjectTask
        {
            Id = 1,
            Project = new Project { UserId = 2 }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(task);

        _repositoryMock.Setup(r => r.CanUserModifyTaskAsync(1, 1))
            .ReturnsAsync(false);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.UpdateTaskAsync(1, "New", null, null));
    }

    // Delete
    [Fact]
    public async Task DeleteTaskAsync_Should_Remove_Task()
    {
        // Arrange
        var task = new ProjectTask
        {
            Id = 1,
            ProjectId = 5,
            Project = new Project { UserId = 1 }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(task);

        _repositoryMock.Setup(r => r.CanUserModifyTaskAsync(1, 1))
            .ReturnsAsync(true);

        // Act
        await _service.DeleteTaskAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.Remove(task), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}