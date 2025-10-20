using Microsoft.AspNetCore.Identity.UI.Services;

namespace Server.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Implement your email sending logic here.
            // This could involve using an SMTP client or a third-party email service API.
            Console.WriteLine($"Sending email to {email} with subject '{subject}' and message: {message}\n");
            return Task.CompletedTask;
        }
    }
}
