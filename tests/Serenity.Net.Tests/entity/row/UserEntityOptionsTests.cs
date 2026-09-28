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

    [Fact]
    public void ClearingRowType_ResetsInferredIdMetadataToDefaults()
    {
        var options = new UserEntityOptions { RowType = typeof(OriginPropertyTests.ConfiguredUserRow) };

        options.RowType = null;

        Assert.Null(options.RowType);
        Assert.Equal("Users", options.TableName);
        Assert.Equal("UserId", options.IdColumnName);
        Assert.Null(options.IdColumnSize);
        Assert.Equal(typeof(Int32Field), options.IdFieldType);
    }

    [Fact]
    public void SettingNullRowTypeOnFreshOptions_PreservesCustomIdMetadata()
    {
        var options = new UserEntityOptions
        {
            IdColumnName = "CustomUserKey",
            IdColumnSize = 80,
            IdFieldType = typeof(GuidField)
        };

        options.RowType = null;

        Assert.Equal("CustomUserKey", options.IdColumnName);
        Assert.Equal(80, options.IdColumnSize);
        Assert.Equal(typeof(GuidField), options.IdFieldType);
    }

    [Fact]
    public void SettingInvalidRowType_DoesNotPartiallyUpdateIdMetadata()
    {
        var options = new UserEntityOptions { RowType = typeof(OriginPropertyTests.ConfiguredUserRow) };

        Assert.Throws<ArgumentException>(() => options.RowType = typeof(UnsupportedIdUserRow));

        Assert.Same(typeof(OriginPropertyTests.ConfiguredUserRow), options.RowType);
        Assert.Equal("UserKey", options.IdColumnName);
        Assert.Equal(40, options.IdColumnSize);
        Assert.Equal(typeof(StringField), options.IdFieldType);
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