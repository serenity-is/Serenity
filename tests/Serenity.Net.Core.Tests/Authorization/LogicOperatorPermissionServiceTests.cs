namespace Serenity.Tests.Authorization;

public class LogicOperatorPermissionServiceTests
{
    [Fact]
    public void ShouldDelegateSimplePermissionsToUnderlyingOne()
    {
        var expected = false;
        var ps = new MockPermissions(p => expected);

        var lops = new LogicOperatorPermissionService(ps);
        Assert.False(lops.HasPermission(""));
        Assert.False(lops.HasPermission("A"));
        Assert.False(lops.HasPermission("B:C"));

        expected = true;
        Assert.True(lops.HasPermission(""));
        Assert.True(lops.HasPermission("A"));
        Assert.True(lops.HasPermission("B:C"));
    }

    private static IPermissionService TYPermissions()
    {
        return new MockPermissions(p => p == "T" || p == "Y");
    }

    [Fact]
    public void ReturnsFalseForOrWhenAllFalse()
    {
        var lops = new LogicOperatorPermissionService(TYPermissions());
        Assert.False(lops.HasPermission("F|N"));
        Assert.False(lops.HasPermission("N|F"));
        Assert.False(lops.HasPermission("N|N|N"));
        Assert.False(lops.HasPermission("N|N|F|F"));
        Assert.False(lops.HasPermission("N|F|N|F"));
    }

    [Fact]
    public void ReturnsTrueForOrWhenAllTrue()
    {
        var lops = new LogicOperatorPermissionService(TYPermissions());
        Assert.True(lops.HasPermission("T|Y"));
        Assert.True(lops.HasPermission("Y|T"));
        Assert.True(lops.HasPermission("Y|Y|Y"));
        Assert.True(lops.HasPermission("Y|Y|T|T"));
        Assert.True(lops.HasPermission("Y|T|Y|T"));
    }

    [Fact]
    public void ReturnsTrueForOrWhenSomeTrue()
    {
        var lops = new LogicOperatorPermissionService(TYPermissions());
        Assert.True(lops.HasPermission("T|F"));
        Assert.True(lops.HasPermission("Y|N"));
        Assert.True(lops.HasPermission("N|Y|N"));
        Assert.True(lops.HasPermission("F|F|T|T"));
        Assert.True(lops.HasPermission("N|Y|N|F|N|N"));
    }

    [Fact]
    public void ReturnsFalseForAndWhenAllFalse()
    {
        var lops = new LogicOperatorPermissionService(TYPermissions());
        Assert.False(lops.HasPermission("F&N"));
        Assert.False(lops.HasPermission("N&F"));
        Assert.False(lops.HasPermission("N&N&N"));
        Assert.False(lops.HasPermission("N&N&F&F"));
        Assert.False(lops.HasPermission("N&F&N&F"));
    }

    [Fact]
    public void ReturnsTrueForAndWhenAllTrue()
    {
        var lops = new LogicOperatorPermissionService(TYPermissions());
        Assert.True(lops.HasPermission("T&Y"));
        Assert.True(lops.HasPermission("Y&T"));
        Assert.True(lops.HasPermission("Y&Y&Y"));
        Assert.True(lops.HasPermission("Y&Y&T&T"));
        Assert.True(lops.HasPermission("Y&T&Y&T"));
    }

    [Fact]
    public void ReturnsFalseForAndWhenSomeFalse()
    {
        var lops = new LogicOperatorPermissionService(TYPermissions());
        Assert.False(lops.HasPermission("T&F"));
        Assert.False(lops.HasPermission("Y&N"));
        Assert.False(lops.HasPermission("T&Y&N"));
        Assert.False(lops.HasPermission("T&T&F&T"));
        Assert.False(lops.HasPermission("N&Y&N&F&N&N"));
    }

    [Fact]
    public void AndTakesPrecedenceOverOr()
    {
        var lops = new LogicOperatorPermissionService(TYPermissions());
        Assert.False(lops.HasPermission("F|T&F"));
        Assert.False(lops.HasPermission("F|F&T"));
        Assert.True(lops.HasPermission("T|F&T"));
        Assert.False(lops.HasPermission("F&T|F&T"));
        Assert.False(lops.HasPermission("T&F|F|F&T"));
        Assert.False(lops.HasPermission("T&T&T&F|F&F&F&T"));
        Assert.True(lops.HasPermission("T&T&T&F|T&T|F&F&F&T"));
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
    [InlineData("(!(!Module:0 | (Module:1 | !0) & !Module:1 | (!(Module:Permission:0 & Module:SubModule:0) & Module:SubModule:0)))", false)]
    public void Evaluates_Expression_As_Expected(string permission, bool expected)
    {
        var lops = new LogicOperatorPermissionService(new MockPermissions(p => p != null && p.Contains("1", StringComparison.Ordinal)));
        var actual = lops.HasPermission(permission);
        if (expected)
            Assert.True(actual);
        else
            Assert.False(actual);
    }
}