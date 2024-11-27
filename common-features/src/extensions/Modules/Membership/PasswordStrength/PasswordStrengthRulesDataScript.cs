namespace Serenity.Extensions;

/// <summary>
/// This declares a dynamic script with key 'PasswordStrengthRules' that will be available from client side.
/// </summary>
[DataScript("PasswordStrengthRules", CacheDuration = -1, Permission = "*")]
public class PasswordStrengthRulesDataScript(IOptions<MembershipSettings> membershipSettings = null) 
    : DataScript<PasswordStrengthRules>
{
    private readonly IOptions<MembershipSettings> membershipSettings = membershipSettings 
        ?? Options.Create(new MembershipSettings());

    protected override PasswordStrengthRules GetData()
    {
        return new PasswordStrengthRules()
        {
            MinPasswordLength = membershipSettings.Value.MinPasswordLength,
            RequireDigit = membershipSettings.Value.RequireDigit,
            RequireLowercase = membershipSettings.Value.RequireLowercase,
            RequireNonAlphanumeric = membershipSettings.Value.RequireNonAlphanumeric,
            RequireUppercase = membershipSettings.Value.RequireUppercase
        };
    }
}
