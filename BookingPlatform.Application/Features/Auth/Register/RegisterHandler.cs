using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace BookingPlatform.Application.Features.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;

    public RegisterHandler(IUserRepository users, IRoleRepository roles)
    {
               _users = users;
               _roles = roles;
    }
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand command, CancellationToken ct)
    {

        if (await _users.EmailExistsAsync(command.Email))
            return Errors.EmailAlreadyExists;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(command.Password),
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsActive = true,
            UserRoles = new List<UserRole>()
        };

        var guestRole = await _roles.GetByNameAsync("Guest", ct);
        if (guestRole != null)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = guestRole.Id,
                AssignedAt = DateTime.UtcNow
            });
        }
        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);
        return new RegisterResponse(user.Id);
    }
}
