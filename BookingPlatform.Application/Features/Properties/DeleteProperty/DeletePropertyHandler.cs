using BookingPlatform.Application.Common;
using BookingPlatform.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Features.Properties.DeleteProperty;

public class DeletePropertyHandler : IRequestHandler<DeletePropertyCommand, Result>
{
    private readonly IPropertyRepository _properties;
    private readonly ICurrentUserService _currentUser;

    public DeletePropertyHandler(IPropertyRepository properties, ICurrentUserService currentUser)
    {
        _properties = properties;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeletePropertyCommand request, CancellationToken ct)
    {
        var prop = await _properties.GetByIdAsync(request.PropertyId, ct);
        if (prop == null) return Errors.PropertyNotFound;

        var currentUserId = _currentUser.UserId;
        if (currentUserId == null) return Errors.NotAuthenticated;

        if (prop.OwnerProfileId != currentUserId.Value) return Errors.NotAuthorized;

        await _properties.DeleteAsync(request.PropertyId, ct);
        await _properties.SaveChangesAsync(ct);

        return Result.Success();
    }
}
