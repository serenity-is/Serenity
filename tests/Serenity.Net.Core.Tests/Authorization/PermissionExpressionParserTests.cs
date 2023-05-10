namespace Serenity.Tests.Authorization;

public class PermissionExpressionParserTests
{
    private bool HasPermission(string permission)
    {
        return permission != null && permission.Contains("1", StringComparison.CurrentCulture);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("!1", false)]
    [InlineData("!0", true)]
    [InlineData("0&0", false)]
    [InlineData("0&1", false)]
    [InlineData("1&0", false)]
    [InlineData("1&1", true)]
    [InlineData("0|0", false)]
    [InlineData("0|1", true)]
    [InlineData("1|0", true)]
    [InlineData("1|1", true)]
    [InlineData("!0&0", false)]
    [InlineData("0&!0", false)]
    [InlineData("!0&!0", true)]
    [InlineData("!0&1", true)]
    [InlineData("0&!1", false)]
    [InlineData("!0&!1", false)]
    [InlineData("!1&0", false)]
    [InlineData("!1&!0", false)]
    [InlineData("1&!0", true)]
    [InlineData("!1&1", false)]
    [InlineData("!1&!1", false)]
    [InlineData("1&!1", false)]
    [InlineData("!0|0", true)]
    [InlineData("!0|!0", true)]
    [InlineData("0|!0", true)]
    [InlineData("!0|1", true)]
    [InlineData("!0|!1", true)]
    [InlineData("0|!1", false)]
    [InlineData("!1|0", false)]
    [InlineData("!1|!0", true)]
    [InlineData("1|!0", true)]
    [InlineData("!1|1", true)]
    [InlineData("!1|!1", false)]
    [InlineData("1|!1", true)]
    [InlineData("(1)", true)]
    [InlineData("(0)", false)]
    [InlineData("(1|1)", true)]
    [InlineData("(1|0)", true)]
    [InlineData("(1&0)", false)]
    [InlineData("1 | 1 & !1", true)]
    [InlineData("(1 | 1) & !1", false)]
    [InlineData("!(0 | 0) & !1", false)]
    [InlineData("(1 | !0) & !1 | !(0 & 0)", true)]
    [InlineData("(1 | !0) & !1 | (!(0 & 0) & 0)", false)]
    [InlineData("(!0 | (1 | !0) & !1 | (!(0 & 0) & 0))", true)]
    [InlineData("(!(!0 | (1 | !0) & !1 | (!(0 & 0) & 0)))", false)]
    public void Evaluate_Expressions(string expression, bool expected)
    {
        var tokens = PermissionExpressionParser.Tokenize(expression);
        var rpn = PermissionExpressionParser.ShuntingYard(tokens);
        var actual = PermissionExpressionParser.Evaluate(rpn, HasPermission);
        if (expected)
            Assert.True(actual);
        else
            Assert.False(actual);
    }

    [Theory]
    [InlineData("!")]
    [InlineData("&")]
    [InlineData("|")]
    [InlineData(" ")]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("!:&")]
    [InlineData("A !! B")]
    [InlineData("A B")]
    [InlineData("A && B")]
    [InlineData("(&) | B & ||")]
    public void Evaluate_InvalidExpressionThrows(string expression)
    {
        var tokens = PermissionExpressionParser.Tokenize(expression);
        var rpn = PermissionExpressionParser.ShuntingYard(tokens);
        Assert.Throws<InvalidOperationException>(() =>
        {
            PermissionExpressionParser.Evaluate(rpn, HasPermission);
        });
    }
}