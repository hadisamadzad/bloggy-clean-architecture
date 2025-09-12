namespace Bloggy.Identity.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailByTemplateIdAsync(long templateId, List<string> recipients,
        Dictionary<string, string> parameters);
}