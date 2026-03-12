using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Users.UpdateMe;

public record UpdateMeCommand(string FirstName, string LastName, string? PhoneNumber) : IRequest<Result<bool>>;
