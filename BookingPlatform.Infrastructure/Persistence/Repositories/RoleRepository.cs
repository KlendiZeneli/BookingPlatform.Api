using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace BookingPlatform.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _db;

    public RoleRepository(AppDbContext db) => _db = db;

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken ct)
        => await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);

    public Task<Role?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct)
        => _db.Roles.ToListAsync(ct).ContinueWith(t => (IEnumerable<Role>)t.Result, ct);

    public Task UpdateAsync(Role entity, CancellationToken ct)
    {
        _db.Roles.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var r = await _db.Roles.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (r != null) _db.Roles.Remove(r);
    }

    public async Task AddAsync(Role role, CancellationToken ct)
    {
        await _db.Roles.AddAsync(role, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
