using System.Threading.Tasks;
using faceaccess;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace VueJsAspNetCoreSample.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public void SendEmailAsync(string email, string subject, string textMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Setting.EmailSenderName, Setting.EmailSenderEmailForAuth));
            message.To.Add(new MailboxAddress("",email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = textMessage
            };
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(Setting.EmailSenderHost, 587, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(Setting.EmailSenderEmailForAuth, Setting.EmailSenderPasswordFromMail);

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