using TaskTracker.Domain.Models;
using TaskTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

// Repository for database User access
public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(User user) =>
    await _context.Users.AddAsync(user);
    public async Task<User?> GetByLoginAsync(string login) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Login == login);

    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
    public async Task<bool> ExistsAsync(string login) =>
    await _context.Users.AnyAsync(u => u.Login == login);
}