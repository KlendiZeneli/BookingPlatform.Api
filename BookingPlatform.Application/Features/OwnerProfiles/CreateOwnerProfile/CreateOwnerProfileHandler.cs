using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Events;
using BookingPlatform.Application.Common.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.OwnerProfiles.CreateOwnerProfile;

public class CreateOwnerProfileHandler : IRequestHandler<CreateOwnerProfileCommand, Result<CreateOwnerProfileResponse>>
{
    private readonly IOwnerProfileRepository _repo;
    private readonly IEventProducer _events;

    public CreateOwnerProfileHandler(IOwnerProfileRepository repo, IEventProducer events)
    {
        _repo = repo;
        _events = events;
    }

    public async Task<Result<CreateOwnerProfileResponse>> Handle(CreateOwnerProfileCommand request, CancellationToken ct)
    {
        var existing = await _repo.GetByUserIdAsync(request.UserId, ct);
        if (existing != null)
            return Errors.OwnerProfileAlreadyExists;

        var profile = new OwnerProfile
        {
            UserId = request.UserId,
            IdentityCardNumber = request.IdentityCardNumber,
            BusinessName = request.BusinessName,
            CreditCard = request.CreditCard,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(profile, ct);
        await _repo.SaveChangesAsync(ct);

        await _events.ProduceAsync(KafkaTopics.OwnerProfileCreated,
            new OwnerProfileCreatedEvent(profile.UserId, profile.IdentityCardNumber, profile.BusinessName, DateTime.UtcNow), ct);

        return new CreateOwnerProfileResponse(profile.UserId);
    }
}
