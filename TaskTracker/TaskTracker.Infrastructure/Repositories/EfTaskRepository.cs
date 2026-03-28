using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repositories;

// Repository for database Task access (CRUD with db)
public class EfTaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public EfTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    // Write
    public async Task AddAsync(ProjectTask task)
    {
        try
        {
            await _context.ProjectTasks.AddAsync(task);
        }
        catch (Exception ex)
        {
            throw new AppException("DbAddFailed", ex);
        }
    }

    // Read
    public async Task<List<ProjectTask>> GetByProjectIdAsync(int projectId)
    {
        try
        {
            return await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new AppException("DbReadFailed", ex);
        }
    }

    public async Task<ProjectTask?> GetByIdAsync(int taskId)
    {
        try
        {
            return await _context.ProjectTasks
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }
        catch (Exception ex)
        {
            throw new AppException("DbReadFailed", ex);
        }
    }

    // Update
    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new AppException("DbSaveFailed", ex);
        }
    }

    // Delete
    public void Remove(ProjectTask task)
    {
        try
        {
            _context.ProjectTasks.Remove(task);
        }
        catch (Exception ex)
        {
            throw new AppException("DbDeleteFailed", ex);
        }
    }

    // Business
    public async Task<bool> CanUserModifyTaskAsync(int taskId, int userId)
    {
        try
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) return false;

            return task.Project.UserId == userId;
        }
        catch (Exception ex)
        {
            throw new AppException("DbReadFailed", ex);
        }
    }
}