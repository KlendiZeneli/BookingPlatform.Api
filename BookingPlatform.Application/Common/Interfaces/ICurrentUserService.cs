using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
}
