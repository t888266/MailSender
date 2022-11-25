using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MailSender
{
    public interface IMailSender
    {
        public Task<bool> SendMailAsync(string to,string displayName, string subject, string body);
        public Task<bool> SendMailAsync(MailContent content);

    }
    public class SenderMail
    {
        public SenderMail(string mail,string displayName, string password, string host, int port)
        {
            Mail = mail;
            DisplayName = displayName;
            Password = password;
            Host = host;
            Port = port;
        }

        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
    public class MailSender : IMailSender
    {
        Dictionary<string, SenderMail> senderMailCollection = new Dictionary<string, SenderMail>();
        public MailSender()
        {
            senderMailCollection.Add("gmail", new SenderMail("scadaproj@gmail.com","SCDAdmin", "fjbgezsmajxufobv",
                "smtp.gmail.com", 587));
        }
        public async Task<bool> SendMailAsync(string to,string displayName, string subject, string body)
        {
            SenderMail senderMail = senderMailCollection["gmail"];
            var content = new MimeMessage();
            content.Subject = subject;
            content.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = body
            };
            content.From.Add(new MailboxAddress(senderMail.DisplayName, senderMail.Mail));
            content.To.Add(new MailboxAddress(displayName,to));
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(senderMail.Host, senderMail.Port, SecureSocketOptions.StartTls);
                    client.Authenticate(senderMail.Mail, senderMail.Password);
                    await client.SendAsync(content);
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    client.Disconnect(true);
                }
                return false;
            }
        }

        public async Task<bool> SendMailAsync(MailContent content)
        {
            return await SendMailAsync(content.To, content.DisplayName, content.Subject, content.Body);
        }
    }
}
