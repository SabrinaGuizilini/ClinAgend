using ClinAgend.Core.Interfaces;
using System.Text;
using ClinAgend.Models.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ClinAgend.Core.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        using var client = new HttpClient();

        var url = "https://api.brevo.com/v3/smtp/email";

        client.DefaultRequestHeaders.Add("api-key", _settings.ApiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var payload = new
        {
            sender = new { email = _settings.Email, name = "ClinAgend" },
            to = new[] { new { email = to } },
            subject = subject,
            htmlContent = body
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetail = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API do Brevo: {response.StatusCode} - {errorDetail}");
        }
    }
}
