using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace VueJsAspNetCoreSample.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private IConfiguration _configuration;

        public AuthMessageSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmailAsync(string email, string subject, string textMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["EmailSender:Name"], _configuration["EmailSender:EmailForAuth"]));
            message.To.Add(new MailboxAddress("",email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = textMessage
            };
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(_configuration["EmailSender:Host"], 587, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_configuration["EmailSender:EmailForAuth"], _configuration["EmailSender:PasswordFromMail"]);

                client.Send(message);
                client.Disconnect(true);
            }          
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }   
    }
}