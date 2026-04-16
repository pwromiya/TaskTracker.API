using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repositories;

// Repository for database Project access (CRUD with db)
public class EfProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;
    public EfProjectRepository(AppDbContext context)
    {
        _context = context;

    }

    // Write
    public async Task AddAsync(Project project)
    {
        try
        {
            await _context.Projects.AddAsync(project);
        }
        catch (Exception ex)
        {
            throw new AppException("DbAddFailed", ex);
        }
    }

    // Read
    public async Task<Project?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            throw new AppException("DbReadFailed", ex);
        }
    }

    public async Task<List<Project>> GetUserProjectsAsync(int userId)
    {
        try
        {
            return await _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.Tasks)
                .ToListAsync();
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
    public void Remove(Project project)
    {
        try
        {
            _context.Projects.Remove(project);
        }
        catch (Exception ex)
        {
            throw new AppException("DbDeleteFailed", ex);
        }
    }

    public async Task<bool> ExistsAsync(string name, int userId)
    {
        return await _context.Projects.AnyAsync(p => p.Name == name && p.UserId == userId);
    }

}