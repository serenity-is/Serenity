namespace Serenity.Web;

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

    private static MockPermissions TYPermissions()
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
        var lops = new LogicOperatorPermissionService(new MockPermissions(p => p != null && p.Contains('1', StringComparison.Ordinal)));
        var actual = lops.HasPermission(permission);
        if (expected)
            Assert.True(actual);
        else
            Assert.False(actual);
    }

    [Fact]
    public void Grant_ThrowsNotImplementedException_WhenUnderlyingIsNotTransientGrantor()
    {
        var lops = new LogicOperatorPermissionService(new MockPermissions(p => true));
        Assert.Throws<NotImplementedException>(() => lops.Grant("A"));
    }

    [Fact]
    public void GrantAll_ThrowsNotImplementedException_WhenUnderlyingIsNotTransientGrantor()
    {
        var lops = new LogicOperatorPermissionService(new MockPermissions(p => true));
        Assert.Throws<NotImplementedException>(() => lops.GrantAll());
    }

    [Fact]
    public void UndoGrant_ThrowsNotImplementedException_WhenUnderlyingIsNotTransientGrantor()
    {
        var lops = new LogicOperatorPermissionService(new MockPermissions(p => true));
        Assert.Throws<NotImplementedException>(() => lops.UndoGrant());
    }

    [Fact]
    public void IsAllGranted_ReturnsFalse_WhenUnderlyingIsNotTransientGrantor()
    {
        var lops = new LogicOperatorPermissionService(new MockPermissions(p => true));
        Assert.False(lops.IsAllGranted());
    }

    [Fact]
    public void GetGranted_ReturnsEmpty_WhenUnderlyingIsNotTransientGrantor()
    {
        var lops = new LogicOperatorPermissionService(new MockPermissions(p => true));
        Assert.Empty(lops.GetGranted());
    }

    [Fact]
    public void Grant_DelegatesToUnderlyingTransientGrantor()
    {
        var underlying = new TransientGrantingPermissionService(new MockPermissions(p => true));
        var lops = new LogicOperatorPermissionService(underlying);

        lops.Grant("A");
        Assert.True(lops.HasPermission("A"));
    }

    [Fact]
    public void GrantAll_DelegatesToUnderlyingTransientGrantor()
    {
        var underlying = new TransientGrantingPermissionService(new MockPermissions(p => true));
        var lops = new LogicOperatorPermissionService(underlying);

        lops.GrantAll();
        Assert.True(lops.IsAllGranted());
    }

    [Fact]
    public void UndoGrant_DelegatesToUnderlyingTransientGrantor()
    {
        var underlying = new TransientGrantingPermissionService(new MockPermissions(p => false));
        var lops = new LogicOperatorPermissionService(underlying);

        lops.Grant("A");
        Assert.True(lops.HasPermission("A"));
        lops.UndoGrant();
        Assert.False(lops.HasPermission("A"));
    }

    [Fact]
    public void IsAllGranted_DelegatesToUnderlyingTransientGrantor()
    {
        var underlying = new TransientGrantingPermissionService(new MockPermissions(p => true));
        var lops = new LogicOperatorPermissionService(underlying);

        Assert.False(lops.IsAllGranted());
        underlying.GrantAll();
        Assert.True(lops.IsAllGranted());
    }

    [Fact]
    public void GetGranted_DelegatesToUnderlyingTransientGrantor()
    {
        var underlying = new TransientGrantingPermissionService(new MockPermissions(p => true));
        var lops = new LogicOperatorPermissionService(underlying);

        lops.Grant("A", "B");
        var granted = lops.GetGranted().ToList();
        Assert.Contains("A", granted);
        Assert.Contains("B", granted);
    }
}