namespace Serenity.Extensions;

public class MigrationAttributesTests
{
    [Fact]
    public void MigrationKeyAttribute_Accepts_Valid_Format()
    {
        var attr = new MigrationKeyAttribute(20240101_0000);
        Assert.True(attr.Version > 0);
    }

    [Fact]
    public void MigrationKeyAttribute_Accepts_Seconds_Format()
    {
        var attr = new MigrationKeyAttribute(20240101_000000);
        Assert.True(attr.Version > 0);
    }

    [Fact]
    public void MigrationKeyAttribute_Throws_For_Invalid_Version()
    {
        Assert.Throws<Exception>(() => new MigrationKeyAttribute(1));
    }

    [Fact]
    public void DefaultDBAttribute_Can_Be_Created()
    {
        Assert.IsAssignableFrom<FluentMigrator.TagsAttribute>(new DefaultDBAttribute());
    }

    [Fact]
    public void TargetDBAttribute_Can_Be_Created()
    {
        Assert.IsAssignableFrom<FluentMigrator.TagsAttribute>(new TargetDBAttribute("SqlServer"));
    }
}
