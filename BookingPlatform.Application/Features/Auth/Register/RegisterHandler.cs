using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace BookingPlatform.Application.Features.Auth.Register;

public class RegisterHandler
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;

    public RegisterHandler(IUserRepository users, IRoleRepository roles)
    {
               _users = users;
               _roles = roles;
    }
    public async Task<RegisterResponse> Handle(RegisterCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.FirstName) ||
            string.IsNullOrWhiteSpace(command.LastName) ||
            string.IsNullOrWhiteSpace(command.Email) ||
            string.IsNullOrWhiteSpace(command.Password))
        {
            throw new Exception("All fields are required.");
        }
        if (!IsValidEmail(command.Email))
        {
            throw new Exception("Invalid email format.");
        }

        if (!IsValidPassword(command.Password))
            throw new Exception("Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.");
        if (await _users.EmailExistsAsync(command.Email))
        {
            throw new Exception("Email already exists");
        }
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(command.Password),
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            IsActive = true
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

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 8)
            return false;

        if (!Regex.IsMatch(password, "[A-Z]"))
            return false;

        if (!Regex.IsMatch(password, "[a-z]"))
            return false;

        if (!Regex.IsMatch(password, "[0-9]"))
            return false;

        if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            return false;

        return true;
    }
}
