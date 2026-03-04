using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common;

public static class Errors
{
    public static Error UserNotFound { get; } = new("UserNotFound", ErrorType.NotFound, "User does not exist.");
    public static Error InvalidCredentials { get; } = new("InvalidCredentials", ErrorType.Unauthorized, "Username or password is incorrect.");
    public static Error TokenGenerationFailed { get; } = new("TokenGenerationFailed", ErrorType.ServerError, "Failed to generate JWT token.");

    public static Error EmailAlreadyExists { get; } = new("EmailAlreadyExists",ErrorType.AlreadyExists, "An account with this email already exists.");

    public static Error FieldsRequired { get; } = new("FieldsRequired", ErrorType.EmailFormat, "All fields are required.");

    public static Error EmailFormat { get; } = new("EmailFormat", ErrorType.EmailFormat, "The email address format is invalid.");

    public static Error PasswordFormat { get; } = new("PasswordFormat", ErrorType.EmailFormat, "Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.");

    public static Error Unknown { get; } = new("Unknown", ErrorType.ServerError, "An unexpected error occurred.");
}