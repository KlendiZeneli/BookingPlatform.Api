namespace BookingPlatform.Application.Common.Events;

public static class KafkaTopics
{
    public const string UserRegistered = "user-registered";
    public const string PasswordResetRequested = "password-reset-requested";
    public const string PasswordChanged = "password-changed";
    public const string PasswordResetCompleted = "password-reset-completed";

    public const string OwnerProfileCreated = "owner-profile-created";
    public const string OwnerProfileVerified = "owner-profile-verified";

    public const string PropertyCreated = "property-created";
    public const string PropertyUpdated = "property-updated";
    public const string PropertyDeleted = "property-deleted";

    public const string BookingCreated = "booking-created";
    public const string BookingStatusChanged = "booking-status-changed";

    public const string ReviewCreated = "review-created";
}

public record UserRegisteredEvent(Guid UserId, string Email, string FirstName, string LastName, DateTime OccurredAtUtc);

public record PasswordResetRequestedEvent(Guid UserId, string Email, string FirstName, string Token, DateTime ExpiresAtUtc, string ResetUrl, DateTime OccurredAtUtc);
public record PasswordChangedEvent(Guid UserId, string Email, DateTime OccurredAtUtc);
public record PasswordResetCompletedEvent(Guid UserId, string Email, DateTime OccurredAtUtc);

public record OwnerProfileCreatedEvent(Guid UserId, string IdentityCardNumber, string? BusinessName, DateTime OccurredAtUtc);
public record OwnerProfileVerifiedEvent(Guid UserId, bool Approved, string? Notes, DateTime? VerifiedAtUtc, DateTime OccurredAtUtc);

public record PropertyCreatedEvent(Guid PropertyId, Guid OwnerProfileId, string Name, decimal PricePerNight, DateTime OccurredAtUtc);
public record PropertyUpdatedEvent(Guid PropertyId, Guid OwnerProfileId, string Name, bool IsActive, DateTime OccurredAtUtc);
public record PropertyDeletedEvent(Guid PropertyId, Guid OwnerProfileId, DateTime OccurredAtUtc);

public record BookingCreatedEvent(Guid BookingId, Guid PropertyId, Guid GuestId, DateTime StartDate, DateTime EndDate, int GuestCount, decimal TotalPrice, DateTime OccurredAtUtc);
public record BookingStatusChangedEvent(Guid BookingId, Guid PropertyId, Guid GuestId, string OldStatus, string NewStatus, DateTime OccurredAtUtc);

public record ReviewCreatedEvent(Guid ReviewId, Guid BookingId, Guid PropertyId, Guid GuestId, int Rating, DateTime OccurredAtUtc);
