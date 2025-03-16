using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Middleware.Email
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var enableSSL = bool.Parse(_configuration["EmailSettings:EnableSSL"]);

            using MailMessage mail = new();
            mail.To.Add(toEmail);
            mail.From = new MailAddress(senderEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using SmtpClient smtp = new(smtpServer, port);
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = enableSSL;

            await smtp.SendMailAsync(mail); 
        }
    }
}
