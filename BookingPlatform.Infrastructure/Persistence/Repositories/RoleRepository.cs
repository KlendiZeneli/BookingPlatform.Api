using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _db;

    public RoleRepository(AppDbContext db) => _db = db;

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken ct)
        => await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
}
