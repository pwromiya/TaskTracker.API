using Moq;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Models;

namespace TaskTracker.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock = new();
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _service = new TaskService(
            _repositoryMock.Object,
            _projectRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_Throws_InvalidTaskStatus_Key()
    {
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new Project { Id = 5, UserId = 1, Name = "P" });

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.CreateTaskAsync("Task", "Desc", 5, 99));

        Assert.Equal("InvalidTaskStatus", ex.UserMessage);
    }

    [Fact]
    public async Task UpdateTaskAsync_Throws_TaskNotFound_Key()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((ProjectTask?)null);

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.UpdateTaskAsync(1, "Name", "Desc", Domain.Models.TaskStatus.Todo, 1));

        Assert.Equal("TaskNotFound", ex.UserMessage);
    }

    [Fact]
    public async Task DeleteTaskAsync_Throws_AccessDenied_Key()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ProjectTask
            {
                Id = 1,
                ProjectId = 2,
                Project = new Project { Id = 2, UserId = 22, Name = "P" },
                Title = "T"
            });
        _repositoryMock.Setup(r => r.CanUserModifyTaskAsync(1, 1))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.DeleteTaskAsync(1, 1));

        Assert.Equal("AccessDenied", ex.UserMessage);
    }

    [Fact]
    public async Task GetByProjectIdForUserAsync_Throws_AccessDenied_For_Foreign_Project()
    {
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new Project { Id = 5, UserId = 99, Name = "Foreign" });

        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _service.GetByProjectIdForUserAsync(5, 1));

        Assert.Equal("AccessDenied", ex.UserMessage);
    }

    [Fact]
    public async Task GetByProjectIdForUserAsync_Returns_Tasks_For_Owner_Project()
    {
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new Project { Id = 5, UserId = 1, Name = "Own" });
        _repositoryMock.Setup(r => r.GetByProjectIdAsync(5))
            .ReturnsAsync(new List<ProjectTask>
            {
                new() { Id = 1, Title = "T1", ProjectId = 5 },
                new() { Id = 2, Title = "T2", ProjectId = 5 }
            });

        var result = await _service.GetByProjectIdForUserAsync(5, 1);

        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(5, t.ProjectId));
    }
}