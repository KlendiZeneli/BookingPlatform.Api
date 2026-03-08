using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using BookingPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BookingPlatform.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    public IEnumerable<string> Roles
    {
        get
        {
            var roleClaims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
            if (roleClaims != null)
            {
                foreach (var claim in roleClaims)
                {
                    yield return claim.Value;
                }
            }
        }
    }
}
