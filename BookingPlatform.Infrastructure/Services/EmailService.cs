using BookingPlatform.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    // Placeholder implementation — replace with real SMTP/third-party provider when needed.
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        Console.WriteLine($"[EmailService] To: {to}; Subject: {subject}; Body: {body}");
        return Task.CompletedTask;
    }
}
