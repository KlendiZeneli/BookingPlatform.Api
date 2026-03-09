using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common;

public static class Errors
{
    public static Error UserNotFound { get; } =
        new("UserNotFound", ErrorType.NotFound, "User does not exist.", 404);

    public static Error OwnerProfileNotFound { get; } =
        new("OwnerProfileNotFound", ErrorType.NotFound, "Owner profile does not exist.", 404);

    public static Error OwnerProfileAlreadyExists { get; } =
        new("OwnerProfileAlreadyExists", ErrorType.AlreadyExists, "Owner profile already exists for this user.", 409);

    public static Error InvalidAmenity { get; } =
        new("InvalidAmenity", ErrorType.Validation, "One or more amenity names are invalid.", 400);

    public static Error PropertyNotFound { get; } =
        new("PropertyNotFound", ErrorType.NotFound, "Property does not exist.", 404);

    public static Error BookingConflict { get; } =
        new("BookingConflict", ErrorType.Conflict, "Requested dates overlap with an existing booking.", 409);

    public static Error NotAuthenticated { get; } =
        new("NotAuthenticated", ErrorType.Unauthorized, "User is not authenticated.", 401);

    public static Error NotAuthorized { get; } =
        new("NotAuthorized", ErrorType.Unauthorized, "User is not authorized to perform this action.", 403);

    public static Error AlreadyReviewed { get; } =
        new("AlreadyReviewed", ErrorType.AlreadyExists, "A review for this booking by this guest already exists.", 409);

    public static Error BookingNotFound { get; } =
        new("BookingNotFound", ErrorType.NotFound, "Booking does not exist.", 404);

    public static Error TooManyGuests { get; } =
        new("TooManyGuests", ErrorType.Validation, "The number of guests is higher than allowed.",400);

    public static Error ReviewNotAllowed { get; } =
        new("ReviewNotAllowed", ErrorType.Validation, "Reviews can only be made for confirmed bookings.", 400);

    public static Error InvalidCredentials { get; } =
        new("InvalidCredentials", ErrorType.Unauthorized, "Username or password is incorrect.", 401);

    public static Error TokenGenerationFailed { get; } =
        new("TokenGenerationFailed", ErrorType.ServerError, "Failed to generate JWT token.", 500);

    public static Error EmailAlreadyExists { get; } =
        new("EmailAlreadyExists", ErrorType.AlreadyExists, "An account with this email already exists.", 409);

    public static Error FieldsRequired { get; } =
        new("FieldsRequired", ErrorType.Validation, "All fields are required.", 400);

    public static Error EmailFormat { get; } =
        new("EmailFormat", ErrorType.Validation, "The email address format is invalid.", 400);

    public static Error PasswordFormat { get; } =
        new("PasswordFormat", ErrorType.Validation, "Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.", 400);

    public static Error Unknown { get; } =
        new("Unknown", ErrorType.ServerError, "An unexpected error occurred.", 500);
}