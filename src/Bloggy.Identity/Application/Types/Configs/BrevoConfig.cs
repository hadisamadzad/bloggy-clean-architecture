namespace Bloggy.Identity.Application.Types.Configs;

public class BrevoConfig
{
    public const string Key = "Brevo";

    public string BaseAddress { get; set; }
    public string SendEmailUri { get; set; }
    public string ApiKey { get; set; }
}