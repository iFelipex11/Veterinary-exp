using System.Net;
using System.Net.Mail;

namespace Veterinary.API.Helpers;

public class MailHelper(IConfiguration configuration) : IMailHelper
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<bool> SendMailAsync(string toName, string toEmail, string subject, string body)
    {
        var from = _configuration["mail:from"];
        var password = _configuration["mail:password"];
        var smtp = _configuration["mail:smtp"];
        var portValue = _configuration["mail:port"];
        var displayName = _configuration["mail:name"] ?? "Veterinary";
        var enableSsl = bool.TryParse(_configuration["mail:enableSsl"], out var sslEnabled) && sslEnabled;

        if (string.IsNullOrWhiteSpace(from) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(smtp) ||
            !int.TryParse(portValue, out var port))
        {
            return false;
        }

        using var message = new MailMessage();
        message.From = new MailAddress(from, displayName);
        message.To.Add(new MailAddress(toEmail, toName));
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(smtp, port)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(from, password)
        };

        try
        {
            await client.SendMailAsync(message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
