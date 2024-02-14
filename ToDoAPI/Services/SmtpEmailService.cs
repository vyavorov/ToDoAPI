using System.Net.Mail;
using System.Net;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Services;

public class SmtpEmailService : IEmailService
{

    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        try
        {
            Console.WriteLine($"Attempting to send email to {to}...");

            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);

            using (var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            })
            using (var mailMessage = new MailMessage
            {
                From = new MailAddress(email),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            })
            {
                mailMessage.To.Add(to);
                await client.SendMailAsync(mailMessage);
            }

            Console.WriteLine("Email sent successfully.");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex}");
            // Optionally, log the exception to a more persistent logging solution
            throw; // Rethrowing the exception might be desirable to handle it further up the call stack.
        }
    }
}
