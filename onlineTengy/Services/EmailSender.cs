using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace onlineTengy.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //return Task.CompletedTask;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false, Credentials = new NetworkCredential("tayyab.bhatti30@gmail.com", "@@@123456789T"),
                 EnableSsl=true
            };

            MailMessage mail = new MailMessage
            {
                From = new MailAddress("tayyab.bhatti30@gmail.com")
            };
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            client.Send(mail);

            return Task.CompletedTask;
        }
    }
}
