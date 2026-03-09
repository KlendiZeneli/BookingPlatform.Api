using BookingPlatform.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IOwnerProfileRepository : ICrudRepository<OwnerProfile>
{
    Task<OwnerProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct);
}
