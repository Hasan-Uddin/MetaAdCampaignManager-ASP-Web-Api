using Application.Abstractions.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services.Email;

internal sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _logger = logger;

        _settings = new EmailSettings
        {
            SmtpServer = configuration["EmailSettings:SmtpServer"]!,
            SmtpPort = configuration.GetValue<int>("EmailSettings:SmtpPort"),
            Username = configuration["EmailSettings:Username"]!,
            Password = configuration["EmailSettings:Password"]!,
            FromEmail = configuration["EmailSettings:FromEmail"]!,
            FromName = configuration["EmailSettings:FromName"]!
        };
    }

    public async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        using MimeMessage mimeMessage = CreateMimeMessage(message);
        await SendMimeMessageAsync(mimeMessage, cancellationToken);
    }

    private MimeMessage CreateMimeMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;

        mimeMessage.Body = new BodyBuilder
        {
            HtmlBody = message.IsHtml ? message.Body : null,
            TextBody = message.IsHtml ? null : message.Body
        }.ToMessageBody();

        return mimeMessage;
    }

    private async Task SendMimeMessageAsync(MimeMessage mimeMessage, CancellationToken cancellationToken)
    {
        using var smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            await smtp.SendAsync(mimeMessage, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            //_logger.LogInformation("Email sent to {To}: {Subject}", mimeMessage.To.First(), mimeMessage.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", mimeMessage);
            throw new InvalidOperationException($"Failed to send email to {mimeMessage}", ex);
        }
    }
}
