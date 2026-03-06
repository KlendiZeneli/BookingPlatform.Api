using System;
using System.Collections.Generic;
using System.Text;

namespace BookingPlatform.Application.Common;

public class Error
{
    public string Id { get; }
    public ErrorType Type { get; }
    public string Description { get; }
    public int Code { get; }

    public Error(string id, ErrorType type, string description, int code)
    {
        Id = id;
        Type = type;
        Description = description;
        Code = code;
    }
}