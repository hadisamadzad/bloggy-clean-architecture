namespace Bloggy.Identity.Application.Types.Configs;

public record LockoutConfig
{
    public const string Key = "Lockout";

    public int FailedLoginLimit { get; set; }
    public TimeSpan Duration { get; set; }
}