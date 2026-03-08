using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
