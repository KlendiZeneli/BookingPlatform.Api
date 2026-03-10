using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<bool> EmailExistsAsync(string email)
    {
        return _context.Users.AnyAsync(u => u.Email == email);
    }

    public Task<User?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return _context.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public Task<IEnumerable<User>> GetAllAsync(CancellationToken ct)
    {
        return _context.Users.ToListAsync(ct).ContinueWith(t => (IEnumerable<User>)t.Result, ct);
    }

    public Task UpdateAsync(User entity, CancellationToken ct)
    {
        _context.Users.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user != null)
            _context.Users.Remove(user);
    }

    public Task<User?> GetByEmailAsync(string email,CancellationToken ct)
    {
        return _context.Users
         .Include(u => u.UserRoles)        
             .ThenInclude(ur => ur.Role) 
         .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<User?> GetByResetTokenAsync(string token, CancellationToken ct)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token, ct);
    }
    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}
