using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace VueJsAspNetCoreSample.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public void SendEmailAsync(string email, string subject, string textMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("igor petrov", "igor-red008_91@mail.ru"));
            message.To.Add(new MailboxAddress("",email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = textMessage
            };
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.mail.ru", 587, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("igor-red008_91@mail.ru", "12345678qwertyui");
             
                client.Send(message);
                client.Disconnect(true);
            }          
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
        
        public void SendEmailMessage()
        {
            
        }      
    }
}