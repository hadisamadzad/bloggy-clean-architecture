namespace Identity.Application.Interfaces;

public interface ITransactionalEmailService
{
    Task<bool> SendEmailByTemplateIdAsync(long templateId, List<string> recipients,
        Dictionary<string, string> parameters);
}