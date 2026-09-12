namespace Serenity.Extensions;

public class PasswordStrengthRulesDataScriptTests
{
    [Fact]
    public void GetData_Returns_Rules_From_Settings()
    {
        var settings = new MembershipSettings
        {
            MinPasswordLength = 9,
            RequireDigit = false,
            RequireLowercase = false,
            RequireNonAlphanumeric = false,
            RequireUppercase = false
        };

        var script = new PasswordStrengthRulesDataScript(Options.Create(settings));
        var rules = Assert.IsType<PasswordStrengthRules>(script.GetScriptData());

        Assert.Equal(9, rules.MinPasswordLength);
        Assert.False(rules.RequireDigit);
        Assert.False(rules.RequireLowercase);
        Assert.False(rules.RequireNonAlphanumeric);
        Assert.False(rules.RequireUppercase);
    }

    [Fact]
    public void GetData_Uses_Default_Settings_When_Options_Null()
    {
        var script = new PasswordStrengthRulesDataScript(null);
        var rules = Assert.IsType<PasswordStrengthRules>(script.GetScriptData());

        Assert.Equal(new MembershipSettings().MinPasswordLength, rules.MinPasswordLength);
        Assert.True(rules.RequireDigit);
    }
}
