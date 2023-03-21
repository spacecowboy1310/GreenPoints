using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace GreenPointsAPI.Services.MailService;

public class MailService : IMailService
{

    private readonly IConfiguration _config;
    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendMail(string to, string subject, string body)
    {
        MimeMessage mail = new();
        mail.From.Add(MailboxAddress.Parse($"Greenpoints<{_config.GetSection("Mail").GetSection("Username").Value}>"));
        mail.To.Add(MailboxAddress.Parse(to));
        mail.Subject = subject;
        mail.Body = new TextPart(TextFormat.Html) { Text = body };

        using SmtpClient smtp = new();
        smtp.Connect(_config.GetSection("Mail").GetSection("Host").Value, 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_config.GetSection("Mail").GetSection("Username").Value, _config.GetSection("Mail").GetSection("Password").Value);
        smtp.Send(mail);
        smtp.Disconnect(true);
    }
}
