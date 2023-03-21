namespace GreenPointsAPI.Services.MailService;

public interface IMailService
{
    public void SendMail(string to, string subject, string body);
}
