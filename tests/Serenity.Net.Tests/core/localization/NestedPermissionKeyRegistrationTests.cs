namespace Serenity.Localization;

public class NestedPermissionKeyRegistrationTests
{
    [NestedPermissionKeys]
    [DisplayName("Test Permissions")]
    private class TestPermissions
    {
        [Description("General Permission")]
        public const string General = "General";

        [Description("Group Description")]
        public const string Group = "Group:";

        [Description("Sub Permission")]
        public const string Sub = "Group:Sub";

        public class Nested
        {
            [Description("Nested Permission")]
            public const string NestedKey = "Nested";
        }
    }

    [Fact]
    public void AddNestedPermissions_ThrowsArgumentNullException_WhenTypeSourceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((ILocalTextRegistry)null).AddNestedPermissions(null));
    }

    [Fact]
    public void AddNestedPermissions_ReturnsPermissionKeys()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource(typeof(TestPermissions));

        var permissions = registry.AddNestedPermissions(typeSource);

        Assert.Contains("General", permissions);
        Assert.Contains("Group:Sub", permissions);
        Assert.Contains("Nested", permissions);
    }

    [Fact]
    public void AddNestedPermissions_AddsTextsForPermissions()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource(typeof(TestPermissions));

        registry.AddNestedPermissions(typeSource);

        Assert.Contains((LocalText.InvariantLanguageID, "Permission.General", "General Permission"), registry.AddedList);
        Assert.Contains((LocalText.InvariantLanguageID, "Permission.Group:Sub", "Sub Permission"), registry.AddedList);
        Assert.Contains((LocalText.InvariantLanguageID, "Permission.Nested", "Nested Permission"), registry.AddedList);
    }

    [Fact]
    public void AddNestedPermissions_AddsGroupText()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource(typeof(TestPermissions));

        registry.AddNestedPermissions(typeSource);

        Assert.Contains((LocalText.InvariantLanguageID, "Permission.Group:", "Group Description"), registry.AddedList);
    }

    [Fact]
    public void AddNestedPermissions_AddsGroupDisplayName()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource(typeof(TestPermissions));

        registry.AddNestedPermissions(typeSource);

        // Group:Sub and Group: share the same prefix, so the group display name is added.
        Assert.Contains((LocalText.InvariantLanguageID, "Permission.Group:", "Group Description"), registry.AddedList);
    }

    [NestedPermissionKeys]
    [DisplayName("Group Display")]
    private class GroupPermissions
    {
        public const string A = "Group:A";
        public const string B = "Group:B";
    }

    [NestedPermissionKeys]
    private class MixedPermissions
    {
        public const string A = "A";
        public const int SomeInt = 1;
        public const string Empty = "";
        public const string WithOperator = "A|B";
    }

    [Fact]
    public void AddNestedPermissions_AddsGroupDisplayName_WhenAllKeysSharePrefix()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource(typeof(GroupPermissions));

        registry.AddNestedPermissions(typeSource);

        Assert.Contains((LocalText.InvariantLanguageID, "Permission.Group:", "Group Display"), registry.AddedList);
    }

    [Fact]
    public void AddNestedPermissions_SkipsNonStringAndOperatorKeys()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource(typeof(MixedPermissions));

        var permissions = registry.AddNestedPermissions(typeSource);

        Assert.Contains("A", permissions);
        Assert.DoesNotContain("A|B", permissions);
        Assert.DoesNotContain("", permissions);
    }
}
