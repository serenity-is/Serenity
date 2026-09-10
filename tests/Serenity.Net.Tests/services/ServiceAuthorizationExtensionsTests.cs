namespace Serenity.Data;

public class ServiceAuthorizationExtensionsTests
{
    [ReadPermission("ReadPerm")]
    private class ReadRow
    {
    }

    [InsertPermission("InsertPerm")]
    private class InsertRow
    {
    }

    [UpdatePermission("UpdatePerm")]
    private class UpdateRow
    {
    }

    [DeletePermission("DeletePerm")]
    private class DeleteRow
    {
    }

    [ModifyPermission("ModifyPerm")]
    private class ModifyRow
    {
    }

    private class NoPermissionRow
    {
    }

    private static string? CapturePermission(Func<IPermissionService, ITextLocalizer, string?> action)
    {
        string? requested = null;
        var permissions = new MockPermissions(p => { requested = p; return true; });
        action(permissions, NullTextLocalizer.Instance);
        return requested;
    }

    private static IRequestContext CreateContext(IPermissionService permissions)
    {
        return new DefaultRequestContext(new NullBehaviorProvider(), new NullTwoLevelCache(),
            NullTextLocalizer.Instance, permissions, new NullUserAccessor());
    }

    [Fact]
    public void AuthorizeList_Uses_ReadPermission()
    {
        Assert.Equal("ReadPerm", CapturePermission((p, l) => { p.AuthorizeList<ReadRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeList_On_Context_Uses_ReadPermission()
    {
        string? requested = null;
        var context = CreateContext(new MockPermissions(p => { requested = p; return true; }));
        context.AuthorizeList<ReadRow>();
        Assert.Equal("ReadPerm", requested);
    }

    [Fact]
    public void AuthorizeList_Throws_When_No_Permission_Attribute()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new MockPermissions(p => true).AuthorizeList<NoPermissionRow>(NullTextLocalizer.Instance));
    }

    [Fact]
    public void AuthorizeList_Validates_Permission()
    {
        var permissions = new MockPermissions(p => false);
        var ex = Assert.Throws<ValidationError>(() =>
            permissions.AuthorizeList<ReadRow>(NullTextLocalizer.Instance));
        Assert.Equal("AccessDenied", ex.ErrorCode);
    }

    [Fact]
    public void AuthorizeCreate_Uses_InsertPermission()
    {
        Assert.Equal("InsertPerm", CapturePermission((p, l) => { p.AuthorizeCreate<InsertRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeCreate_Falls_Back_To_ModifyPermission()
    {
        Assert.Equal("ModifyPerm", CapturePermission((p, l) => { p.AuthorizeCreate<ModifyRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeCreate_Falls_Back_To_ReadPermission()
    {
        Assert.Equal("ReadPerm", CapturePermission((p, l) => { p.AuthorizeCreate<ReadRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeCreate_Throws_When_No_Permission_Attribute()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new MockPermissions(p => true).AuthorizeCreate<NoPermissionRow>(NullTextLocalizer.Instance));
    }

    [Fact]
    public void AuthorizeUpdate_Uses_UpdatePermission()
    {
        Assert.Equal("UpdatePerm", CapturePermission((p, l) => { p.AuthorizeUpdate<UpdateRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeUpdate_Falls_Back_To_ModifyPermission()
    {
        Assert.Equal("ModifyPerm", CapturePermission((p, l) => { p.AuthorizeUpdate<ModifyRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeUpdate_Falls_Back_To_ReadPermission()
    {
        Assert.Equal("ReadPerm", CapturePermission((p, l) => { p.AuthorizeUpdate<ReadRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeUpdate_Throws_When_No_Permission_Attribute()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new MockPermissions(p => true).AuthorizeUpdate<NoPermissionRow>(NullTextLocalizer.Instance));
    }

    [Fact]
    public void AuthorizeDelete_Uses_DeletePermission()
    {
        Assert.Equal("DeletePerm", CapturePermission((p, l) => { p.AuthorizeDelete<DeleteRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeDelete_Falls_Back_To_ModifyPermission()
    {
        Assert.Equal("ModifyPerm", CapturePermission((p, l) => { p.AuthorizeDelete<ModifyRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeDelete_Falls_Back_To_ReadPermission()
    {
        Assert.Equal("ReadPerm", CapturePermission((p, l) => { p.AuthorizeDelete<ReadRow>(l); return null; }));
    }

    [Fact]
    public void AuthorizeDelete_Throws_When_No_Permission_Attribute()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new MockPermissions(p => true).AuthorizeDelete<NoPermissionRow>(NullTextLocalizer.Instance));
    }

    [Fact]
    public void AuthorizeCreate_On_Context_Uses_InsertPermission()
    {
        string? requested = null;
        var context = CreateContext(new MockPermissions(p => { requested = p; return true; }));
        context.AuthorizeCreate<InsertRow>();
        Assert.Equal("InsertPerm", requested);
    }

    [Fact]
    public void AuthorizeUpdate_On_Context_Uses_UpdatePermission()
    {
        string? requested = null;
        var context = CreateContext(new MockPermissions(p => { requested = p; return true; }));
        context.AuthorizeUpdate<UpdateRow>();
        Assert.Equal("UpdatePerm", requested);
    }

    [Fact]
    public void AuthorizeDelete_On_Context_Uses_DeletePermission()
    {
        string? requested = null;
        var context = CreateContext(new MockPermissions(p => { requested = p; return true; }));
        context.AuthorizeDelete<DeleteRow>();
        Assert.Equal("DeletePerm", requested);
    }
}
