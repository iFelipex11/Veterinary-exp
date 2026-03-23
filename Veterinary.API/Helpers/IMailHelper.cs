namespace Veterinary.API.Helpers;

public interface IMailHelper
{
    Task<bool> SendMailAsync(string toName, string toEmail, string subject, string body);
}
