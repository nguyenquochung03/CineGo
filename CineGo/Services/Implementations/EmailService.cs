using CineGo.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CineGo.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            string host = smtpSettings["Host"]!;
            int port = int.Parse(smtpSettings["Port"]!);
            string fromEmail = smtpSettings["FromEmail"]!;
            string password = smtpSettings["Password"]!;
            bool enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "CineGo Cinema"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            try
            {
                await smtp.SendMailAsync(mail);
                Console.WriteLine($"Email đã gửi đến: {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gửi email thất bại: {ex.Message}");
                throw;
            }
        }
    }
}