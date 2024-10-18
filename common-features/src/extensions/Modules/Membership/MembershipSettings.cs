namespace Serenity.Extensions;

[DefaultSectionKey(SectionKey)]
public class MembershipSettings
{
    public const string SectionKey = "Membership";

    public int MinPasswordLength { get; set; } = 6;
    public bool RequireDigit { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireNonAlphanumeric { get; set; } = true;
    public bool RequireUppercase { get; set; } = true;
    public int SaltSize { get; set; } = 5;
}
