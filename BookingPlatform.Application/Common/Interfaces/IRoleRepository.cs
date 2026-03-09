using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IRoleRepository : ICrudRepository<Role>
{
    Task<Role?> GetByNameAsync(string roleName, CancellationToken ct);
}
