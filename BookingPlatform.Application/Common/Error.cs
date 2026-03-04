using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common;

public record Error(string Id, ErrorType Type, string Description);