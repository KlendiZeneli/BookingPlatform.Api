using System;
using System.Collections.Generic;

namespace BookingPlatform.Application.Features.Properties.GetAvailability;

public record GetAvailabilityResponse(
    bool IsAvailable,
    IEnumerable<BlockedPeriod> BlockedPeriods
);

public record BlockedPeriod(DateTime StartDate, DateTime EndDate);
