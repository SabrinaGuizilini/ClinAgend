using ClinAgend.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClinAgend.Models.Settings;
using Microsoft.Extensions.Options;

namespace ClinAgend.Core.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body)
    {
        using var smtp = new SmtpClient(
            _settings.Host,
            _settings.Port);

        smtp.Credentials = new NetworkCredential(
            _settings.Email,
            _settings.Password);

        smtp.EnableSsl = true;

        var mail = new MailMessage(
            from: _settings.Email,
            to,
            subject,
            body);

        mail.IsBodyHtml = true;

        await smtp.SendMailAsync(mail);
    }
}
