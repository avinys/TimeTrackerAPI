using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TimeTrackerAPI.Services.Interfaces;

namespace TimeTrackerAPI.Services
{
    public sealed class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration cfg, ILogger<SmtpEmailService> logger)
        {
            _cfg = cfg; _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody, string? textBody = null)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_cfg["Email:Smtp:FromName"] ?? "TimeTracker",
                                            _cfg["Email:Smtp:FromAddress"]!));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;

            var body = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody ?? StripTags(htmlBody)
            };
            msg.Body = body.ToMessageBody();

            using var client = new SmtpClient();
            var host = _cfg["Email:Smtp:Host"] ?? "smtp.zoho.eu";
            var port = int.TryParse(_cfg["Email:Smtp:Port"], out var p) ? p : 587;
            var username = _cfg["Email:Smtp:Username"];
            var password = _cfg["Email:Smtp:Password"];

            await client.ConnectAsync("smtp.zoho.eu", 465, SecureSocketOptions.SslOnConnect);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            if (!string.IsNullOrEmpty(username))
                await client.AuthenticateAsync(username, password);

            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }

        private static string StripTags(string html) =>
            System.Text.RegularExpressions.Regex.Replace(html ?? "", "<.*?>", "");
    }
}
