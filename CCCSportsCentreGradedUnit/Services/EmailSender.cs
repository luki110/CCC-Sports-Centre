using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CCCSportsCentreGradedUnit.Services
{
    /// <summary>
    /// class to handle sending emails
    /// </summary>
    public class EmailSender : IEmailSender
    {
        public AuthMessageSenderOptions Options { get; set; }

        public EmailSender(IOptions<AuthMessageSenderOptions> emailOptions)
        {
            Options = emailOptions.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                //set email and sender
                From = new EmailAddress("noreply@cccsportcentre.com", "CCCSportsCentre"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
