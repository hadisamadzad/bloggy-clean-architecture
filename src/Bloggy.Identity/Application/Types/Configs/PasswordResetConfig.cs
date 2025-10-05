namespace Bloggy.Identity.Application.Types.Configs;

public record PasswordResetConfig
{
    public const string Key = "PasswordReset";

    public string LinkFormat { get; set; }
    public int LinkLifetimeInDays { get; set; }
    public int BrevoTemplateId { get; set; }
}