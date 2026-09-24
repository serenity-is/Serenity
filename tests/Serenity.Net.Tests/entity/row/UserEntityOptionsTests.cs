namespace Serenity.Data;

public class UserEntityOptionsTests
{
    [Fact]
    public void AssignFrom_CopiesAllSettings()
    {
        var source = new UserEntityOptions
        {
            RowType = typeof(OriginPropertyTests.ConfiguredUserRow),
            TableName = "AccountUsers",
            IdColumnName = "AccountKey",
            IdColumnSize = 64,
            IdFieldType = typeof(StringField)
        };
        var target = new UserEntityOptions();

        target.AssignFrom(source);

        Assert.Same(source.RowType, target.RowType);
        Assert.Equal(source.TableName, target.TableName);
        Assert.Equal(source.IdColumnName, target.IdColumnName);
        Assert.Equal(source.IdColumnSize, target.IdColumnSize);
        Assert.Equal(source.IdFieldType, target.IdFieldType);
    }
}