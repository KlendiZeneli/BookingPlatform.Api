using MediatR;
using BookingPlatform.Application.Common;

namespace BookingPlatform.Application.Features.Properties.GetMyProperties;

public record GetMyPropertiesQuery() : IRequest<Result<GetMyPropertiesResponse>>;
