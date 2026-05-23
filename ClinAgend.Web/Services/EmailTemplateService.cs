using ClinAgend.Core.Interfaces;

public class EmailTemplateService
    : IEmailTemplateService
{
    private readonly IWebHostEnvironment _env;

    public EmailTemplateService(
        IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> GetTemplateAsync(
        string templateName)
    {
        var path = Path.Combine(
            _env.ContentRootPath,
            "EmailTemplates",
            templateName);

        return await File.ReadAllTextAsync(path);
    }
}