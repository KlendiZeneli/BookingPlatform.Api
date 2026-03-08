using BookingPlatform.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IOwnerProfileRepository
{
    Task<OwnerProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(OwnerProfile profile, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
