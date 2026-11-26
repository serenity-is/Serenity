namespace Serenity.Services;

public class PermissionExpressionParserTests
{
    private bool HasPermission(string permission)
    {
        return permission != null && permission.Contains('1', StringComparison.CurrentCulture);
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

    [Fact]
    public void Tokenize_ThrowsArgumentNullException_ForNullExpression()
    {
        Assert.Throws<ArgumentNullException>(() => PermissionExpressionParser.Tokenize(null).ToList());
    }

    [Fact]
    public void ShuntingYard_ThrowsArgumentNullException_ForNullTokens()
    {
        Assert.Throws<ArgumentNullException>(() => PermissionExpressionParser.ShuntingYard(null).ToList());
    }

    [Fact]
    public void Evaluate_ThrowsArgumentNullException_ForNullRpnTokens()
    {
        Assert.Throws<ArgumentNullException>(() => PermissionExpressionParser.Evaluate(null, HasPermission));
    }

    [Fact]
    public void Evaluate_ThrowsArgumentNullException_ForNullHasPermission()
    {
        var tokens = PermissionExpressionParser.Tokenize("A");
        var rpn = PermissionExpressionParser.ShuntingYard(tokens);
        Assert.Throws<ArgumentNullException>(() => PermissionExpressionParser.Evaluate(rpn, null));
    }

    [Theory]
    [InlineData("A", false)]
    [InlineData("Module:Permission", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("A|B", true)]
    [InlineData("A&B", true)]
    [InlineData("!A", true)]
    [InlineData("(A)", true)]
    public void IsExpression_Detects_Operators(string? permission, bool expected)
    {
        Assert.Equal(expected, PermissionExpressionParser.IsExpression(permission));
    }

    [Fact]
    public void HasPermission_Passes_Single_Key_As_Is()
    {
        var checkedKeys = new List<string>();
        Assert.True(PermissionExpressionParser.HasPermission("Module:Key", x => { checkedKeys.Add(x); return true; }));
        Assert.Equal(["Module:Key"], checkedKeys);
    }

    [Fact]
    public void HasPermission_Evaluates_Expression_Per_Key()
    {
        var checkedKeys = new List<string>();
        bool hasKey(string key)
        {
            checkedKeys.Add(key);
            return key == "B";
        }

        Assert.True(PermissionExpressionParser.HasPermission("A|B", hasKey));
        Assert.Contains("A", checkedKeys);
        Assert.Contains("B", checkedKeys);
        Assert.DoesNotContain("A|B", checkedKeys);
        Assert.False(PermissionExpressionParser.HasPermission("A&B", hasKey));
        // a cached expression gives the same result
        Assert.True(PermissionExpressionParser.HasPermission("A|B", hasKey));
    }

    [Fact]
    public void HasPermission_Throws_For_Null_Key_Check()
    {
        Assert.Throws<ArgumentNullException>(() => PermissionExpressionParser.HasPermission("A", null));
    }

    [Theory]
    [InlineData("A|")]
    [InlineData("|")]
    [InlineData("A&&B")]
    [InlineData("!")]
    [InlineData("(A")]
    [InlineData("A)")]
    public void HasPermission_Throws_With_Expression_For_Malformed_Expression(string permission)
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            PermissionExpressionParser.HasPermission(permission, _ => true));
        Assert.Contains("'" + permission + "'", exception.Message, StringComparison.Ordinal);

        // the same message on a second call, e.g. when a parsed expression is cached
        exception = Assert.Throws<InvalidOperationException>(() =>
            PermissionExpressionParser.HasPermission(permission, _ => true));
        Assert.Contains("'" + permission + "'", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void HasPermission_Passes_Null_Or_Empty_Permission_To_Key_Check(string? permission)
    {
        var checkedKeys = new List<string>();
        Assert.False(PermissionExpressionParser.HasPermission(permission, x => { checkedKeys.Add(x); return false; }));
        Assert.Equal([permission], checkedKeys);
    }
}