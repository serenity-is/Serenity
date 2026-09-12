namespace Serenity.Extensions;

public class PasswordStrengthRulesTests
{
    [Fact]
    public void Properties_Can_Be_Set_And_Read()
    {
        var rules = new PasswordStrengthRules
        {
            MinPasswordLength = 8,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true
        };

        Assert.Equal(8, rules.MinPasswordLength);
        Assert.True(rules.RequireDigit);
        Assert.True(rules.RequireLowercase);
        Assert.True(rules.RequireNonAlphanumeric);
        Assert.True(rules.RequireUppercase);
    }
}
