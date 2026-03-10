using BookingPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BookingPlatform.Infrastructure.Email;

public class SendGridEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SendGridEmailService(IConfiguration config)
    {
        _config = config;
    }

    private string LoadTemplate(string templateName)
    {
        // try several locations for the templates: current directory, app base, assembly location
        var fileName = $"{templateName}.html";
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "Email", "Templates", fileName),
            Path.Combine(AppContext.BaseDirectory, "Email", "Templates", fileName),
            Path.Combine(Path.GetDirectoryName(typeof(SendGridEmailService).Assembly.Location) ?? string.Empty, "Email", "Templates", fileName)
        };

        foreach (var p in candidates)
        {
            if (File.Exists(p)) return File.ReadAllText(p);
        }

        throw new FileNotFoundException($"Email template '{fileName}' not found. Searched: {string.Join(";", candidates)}");
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        // try environment variable first, then configuration
        var apiKey = Environment.GetEnvironmentVariable("EMAIL_API_KEY") ?? _config["SendGrid:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("SendGrid API key not configured.");

        var client = new SendGridClient(apiKey);

        var fromEmail = _config["SendGrid:FromEmail"];
        var fromName = _config["SendGrid:FromName"] ?? "BookingPlatform";
        var from = new EmailAddress(fromEmail ?? "no-reply@example.com", fromName);

        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: "", htmlContent: htmlContent);

        await client.SendEmailAsync(msg);
    }

    public async Task SendTemplateEmailAsync(
        string toEmail,
        string subject,
        string templateName,
        Dictionary<string, string> values)
    {
        var html = LoadTemplate(templateName);

        foreach (var value in values)
        {
            html = html.Replace($"{{{{{value.Key}}}}}", value.Value);
        }

        await SendEmailAsync(toEmail, subject, html);
    }
}
