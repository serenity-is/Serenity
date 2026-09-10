namespace Serenity.Data.Schema;

public class SchemaModelsTests
{
    [Fact]
    public void FieldInfo_Properties_Are_Settable()
    {
        var info = new FieldInfo
        {
            FieldName = "ID",
            Size = 4,
            Scale = 2,
            IsPrimaryKey = true,
            IsIdentity = true,
            IsNullable = true,
            PKSchema = "dbo",
            PKTable = "T",
            PKColumn = "ID",
            DataType = "int"
        };

        Assert.Equal("ID", info.FieldName);
        Assert.Equal(4, info.Size);
        Assert.Equal(2, info.Scale);
        Assert.True(info.IsPrimaryKey);
        Assert.True(info.IsIdentity);
        Assert.True(info.IsNullable);
        Assert.Equal("dbo", info.PKSchema);
        Assert.Equal("T", info.PKTable);
        Assert.Equal("ID", info.PKColumn);
        Assert.Equal("int", info.DataType);
    }

    [Fact]
    public void ForeignKeyInfo_Properties_Are_Settable()
    {
        var info = new ForeignKeyInfo
        {
            FKName = "FK_T",
            FKColumn = "T_ID",
            PKSchema = "dbo",
            PKTable = "T",
            PKColumn = "ID"
        };

        Assert.Equal("FK_T", info.FKName);
        Assert.Equal("T_ID", info.FKColumn);
        Assert.Equal("dbo", info.PKSchema);
        Assert.Equal("T", info.PKTable);
        Assert.Equal("ID", info.PKColumn);
    }

    [Fact]
    public void TableName_Properties_And_Tablename_Without_Schema()
    {
        var table = new TableName
        {
            Table = "T",
            IsView = true
        };

        Assert.Null(table.Schema);
        Assert.Equal("T", table.Table);
        Assert.True(table.IsView);
        Assert.Equal("T", table.Tablename);
    }

    [Fact]
    public void TableName_Tablename_With_Schema()
    {
        var table = new TableName
        {
            Schema = "dbo",
            Table = "T"
        };

        Assert.Equal("dbo.T", table.Tablename);
    }

    [Fact]
    public void TableName_Tablename_With_Empty_Schema()
    {
        var table = new TableName
        {
            Schema = "",
            Table = "T"
        };

        Assert.Equal("T", table.Tablename);
    }
}
