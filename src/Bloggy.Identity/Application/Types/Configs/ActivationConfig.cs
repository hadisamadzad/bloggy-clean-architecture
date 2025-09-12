namespace Bloggy.Identity.Application.Types.Configs;

public record ActivationConfig
{
    public const string Key = "Activation";

    public string LinkFormat { get; set; }
    public int LinkLifetimeInDays { get; set; }
    public int BrevoTemplateId { get; set; }
}