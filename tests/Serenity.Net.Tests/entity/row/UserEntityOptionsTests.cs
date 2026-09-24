namespace Serenity.Data;

public class UserEntityOptionsTests
{
    private class IntUserRow : Row<IntUserRow.RowFields>
    {
        public class RowFields : RowFieldsBase;

        [IdProperty]
        public int? Id { get; set; }
    }

    private class LongUserRow : Row<LongUserRow.RowFields>
    {
        public class RowFields : RowFieldsBase;

        [IdProperty]
        public long? Id { get; set; }
    }

    private class GuidUserRow : Row<GuidUserRow.RowFields>
    {
        public class RowFields : RowFieldsBase;

        [IdProperty]
        public Guid? Id { get; set; }
    }

    private class NoIdUserRow : Row<NoIdUserRow.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }

    private class UnsupportedIdUserRow : Row<UnsupportedIdUserRow.RowFields>
    {
        public class RowFields : RowFieldsBase;

        [IdProperty]
        public decimal? Id { get; set; }
    }

    [Theory]
    [InlineData(typeof(IntUserRow), typeof(Int32Field))]
    [InlineData(typeof(LongUserRow), typeof(Int64Field))]
    [InlineData(typeof(GuidUserRow), typeof(GuidField))]
    [InlineData(typeof(OriginPropertyTests.ConfiguredUserRow), typeof(StringField))]
    public void RowType_InfersIdFieldType(Type rowType, Type expectedFieldType)
    {
        var options = new UserEntityOptions { RowType = rowType };

        Assert.Equal(expectedFieldType, options.IdFieldType);
    }

    [Fact]
    public void RowType_InfersColumnMetadataAndTableName()
    {
        var options = new UserEntityOptions
        {
            RowType = typeof(OriginPropertyTests.ConfiguredUserRow)
        };

        Assert.Equal("UsersFromRow", options.TableName);
        Assert.Equal("UserKey", options.IdColumnName);
        Assert.Equal(40, options.IdColumnSize);
    }

    [Fact]
    public void RowType_UsesDefaultsWhenMetadataIsMissing()
    {
        var options = new UserEntityOptions { RowType = typeof(IntUserRow) };

        Assert.Equal("Users", options.TableName);
        Assert.Equal("Id", options.IdColumnName);
        Assert.Null(options.IdColumnSize);
    }

    [Theory]
    [InlineData(typeof(string), "UserRowSettings.RowType must be a type that implements IRow.")]
    [InlineData(typeof(NoIdUserRow), "UserRowSettings.RowType must have a property with the [IdProperty] attribute.")]
    [InlineData(typeof(UnsupportedIdUserRow), "UserRowSettings.RowType must have an Id property of type int, long, Guid or string.")]
    public void RowType_RejectsInvalidTypes(Type rowType, string expectedMessage)
    {
        var exception = Assert.Throws<ArgumentException>(() => new UserEntityOptions { RowType = rowType });

        Assert.Equal(expectedMessage, exception.Message);
    }

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