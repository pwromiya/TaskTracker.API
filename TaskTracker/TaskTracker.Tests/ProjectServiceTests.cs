using Moq;
using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Application.Services;
using Wpf_Task4.Domain.Models;
using Wpf_Task4.Domain.Common;

// Testing ProjectService from Application layer
public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ILoggerService> _loggerMock;

    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILoggerService>();

        _currentUserMock.Setup(c => c.CurrentUser)
            .Returns(new User { Id = 1 });

        _service = new ProjectService(
            _repositoryMock.Object,
            _currentUserMock.Object,
            _loggerMock.Object);
    }

    // Create
    [Fact]
    public async Task CreateProjectAsync_Should_Create_Project()
    {
        // Act
        var result = await _service.CreateProjectAsync("Test", "Desc");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Equal(1, result.UserId);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task CreateProjectAsync_Should_Throw_When_Name_Empty()
    {
        await Assert.ThrowsAsync<AppException>(() =>
            _service.CreateProjectAsync("", "Desc"));
    }

    // Update
    [Fact]
    public async Task UpdateProjectAsync_Should_Update_Name()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "Old",
            UserId = 1
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(project);

        // Act
        await _service.UpdateProjectAsync(1, "New", null);

        // Assert
        Assert.Equal("New", project.Name);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectAsync_Should_Throw_When_NotOwner()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Name = "Old",
            UserId = 2
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(project);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.UpdateProjectAsync(1, "New", null));
    }

    // Delete
    [Fact]
    public async Task DeleteProjectAsync_Should_Remove_Project()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            UserId = 1
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(project);

        // Act
        await _service.DeleteProjectAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.Remove(project), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}