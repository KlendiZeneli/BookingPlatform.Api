using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
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

    public Task<User?> GetByEmailAsync(string email,CancellationToken ct)
    {
        return _context.Users
         .Include(u => u.UserRoles)        
             .ThenInclude(ur => ur.Role) 
         .FirstOrDefaultAsync(u => u.Email == email, ct);
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
