using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName, CancellationToken ct);
}
