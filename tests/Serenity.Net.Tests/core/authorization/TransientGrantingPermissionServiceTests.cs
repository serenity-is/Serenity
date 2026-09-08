namespace Serenity.Web;

public class TransientGrantingPermissionServiceTests
{
    private static MockPermissions Allowed(params string[] allowed)
    {
        var set = new HashSet<string>(allowed, StringComparer.Ordinal);
        return new MockPermissions(p => set.Contains(p));
    }

    [Fact]
    public void HasPermission_ReturnsFalse_WhenPermissionIsNullOrEmpty()
    {
        var service = new TransientGrantingPermissionService(Allowed("A"));
        Assert.False(service.HasPermission(null));
        Assert.False(service.HasPermission(""));
        Assert.False(service.HasPermission(" "));
    }

    [Fact]
    public void HasPermission_DelegatesToUnderlyingService_WhenNoGrantIsActive()
    {
        var service = new TransientGrantingPermissionService(Allowed("A", "B"));
        Assert.True(service.HasPermission("A"));
        Assert.True(service.HasPermission("B"));
        Assert.False(service.HasPermission("C"));
    }

    [Fact]
    public void HasPermission_ReturnsFalse_WhenNoServiceAndNoGrantIsActive()
    {
        var service = new TransientGrantingPermissionService();
        Assert.False(service.HasPermission("A"));
    }

    [Fact]
    public void Grant_PermitsGrantedPermissions_UntilUndone()
    {
        var service = new TransientGrantingPermissionService(Allowed("A"));
        service.Grant("B", "C");
        Assert.True(service.HasPermission("B"));
        Assert.True(service.HasPermission("C"));
        Assert.True(service.HasPermission("A"));

        service.UndoGrant();
        Assert.False(service.HasPermission("B"));
        Assert.False(service.HasPermission("C"));
        Assert.True(service.HasPermission("A"));
    }

    [Fact]
    public void Grant_ThrowsArgumentNullException_WhenPermissionsNullOrEmpty()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        Assert.Throws<ArgumentNullException>(() => service.Grant(null));
        Assert.Throws<ArgumentNullException>(() => service.Grant());
        Assert.Throws<ArgumentNullException>(() => service.Grant(Array.Empty<string>()));
    }

    [Fact]
    public void NestedGrants_MergePermissionsIntoSet()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        service.Grant("A");
        service.Grant("B");
        Assert.True(service.HasPermission("A"));
        Assert.True(service.HasPermission("B"));
        Assert.False(service.HasPermission("C"));

        service.UndoGrant();
        Assert.True(service.HasPermission("A"));
        Assert.False(service.HasPermission("B"));
    }

    [Fact]
    public void GrantAll_GrantsEverything_UntilUndone()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        service.GrantAll();
        Assert.True(service.HasPermission("Whatever"));
        Assert.True(service.IsAllGranted());

        service.UndoGrant();
        Assert.False(service.HasPermission("Whatever"));
        Assert.False(service.IsAllGranted());
    }

    [Fact]
    public void GrantAfterGrantAll_KeepsAllPermissionsGranted()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        service.GrantAll();
        service.Grant("A");
        Assert.True(service.HasPermission("Anything"));
        Assert.True(service.IsAllGranted());
    }

    [Fact]
    public void GrantAllOnTopOfGrant_ThenUndo_RestoresPreviousSet()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        service.Grant("A");
        service.GrantAll();
        Assert.True(service.HasPermission("Z"));

        service.UndoGrant();
        Assert.True(service.HasPermission("A"));
        Assert.False(service.HasPermission("Z"));
    }

    [Fact]
    public void UndoGrant_ThrowsInvalidOperationException_WhenStackIsEmpty()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        Assert.Throws<InvalidOperationException>(() => service.UndoGrant());
    }

    [Fact]
    public void IsAllGranted_ReturnsFalse_ByDefault()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        Assert.False(service.IsAllGranted());
    }

    [Fact]
    public void GetGranted_ReturnsCurrentGrantedSet()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        Assert.Empty(service.GetGranted());

        service.Grant("A", "B");
        var granted = service.GetGranted().ToList();
        Assert.Equal(2, granted.Count);
        Assert.Contains("A", granted);
        Assert.Contains("B", granted);

        service.UndoGrant();
        Assert.Empty(service.GetGranted());
    }

    [Fact]
    public void GetGranted_ReturnsEmpty_WhenGrantAllIsActive()
    {
        var service = new TransientGrantingPermissionService(Allowed());
        service.GrantAll();
        Assert.Empty(service.GetGranted());
    }

    [Fact]
    public void UsesRequestContextItems_WhenRequestContextIsProvided()
    {
        var items = new MockHttpContextItemsAccessor();
        var service = new TransientGrantingPermissionService(Allowed(), items);

        Assert.False(service.HasPermission("A"));
        service.Grant("A");
        Assert.True(service.HasPermission("A"));
        Assert.True(items.Items.ContainsKey("GrantingStack"));
        service.UndoGrant();
        Assert.False(service.HasPermission("A"));
    }

    [Fact]
    public void InstancesSharingRequestContext_ShareTheGrantingStack()
    {
        var items = new MockHttpContextItemsAccessor();
        var service1 = new TransientGrantingPermissionService(Allowed(), items);
        var service2 = new TransientGrantingPermissionService(Allowed("A"), items);

        service1.Grant("B");
        Assert.True(service2.HasPermission("B"));
        Assert.True(service1.HasPermission("B"));

        service2.UndoGrant();
        Assert.False(service1.HasPermission("B"));
    }
}
