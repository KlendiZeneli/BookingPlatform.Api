using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IUserRepository : ICrudRepository<User>
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByResetTokenAsync(string token, CancellationToken ct);
    Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct);
}
