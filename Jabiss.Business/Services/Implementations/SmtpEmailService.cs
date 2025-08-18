using Jabiss.Business.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Jabiss.Business.Services.Interfaces;

namespace Jabiss.Business.Services.Implementations
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
            if (string.IsNullOrEmpty(_settings.FromAddress))
                throw new Exception("FromAddress is not configured!");
        }
        public async Task SendAsync(string to, string subject, string body)
        {
            using var mail = new MailMessage();
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = false;
            mail.From = new MailAddress(_settings.FromAddress, _settings.FromName);

            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.User, _settings.Password)
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
