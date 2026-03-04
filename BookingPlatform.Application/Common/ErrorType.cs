using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common;

public enum ErrorType
{
    NotFound,
    Validation,
    Unauthorized,
    Conflict,
    ServerError,
    AlreadyExists,
    FieldsRequired,
    EmailFormat,
    PasswordFormat
}
