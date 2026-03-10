using System.Threading;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
    Task SendTemplateEmailAsync(
        string toEmail,
        string subject,
        string templateName,
        Dictionary<string, string> values);
}