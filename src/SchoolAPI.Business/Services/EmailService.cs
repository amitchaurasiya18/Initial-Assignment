using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using SchoolAPI.Business.Services.Interfaces;

namespace SchoolAPI.Business.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmail(string toEmail, string subject, string message)
        {
            var fromEmail = "benroman1712345@gmail.com";
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("benroman1712345@gmail.com", "fedj gqax kbls ucgt"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromEmail, toEmail, subject, message);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}