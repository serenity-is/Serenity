namespace Serenity.Data.Schema;

public partial class SchemaProviderIntegrationTests_Sqlite
{
    [Fact]
    public void SqliteSchemaProvider_Reads_Columns_Keys_ForeignKeys_RowId_And_Views()
    {
        if (SqlIntegrationConnections.ShouldSkip("Sqlite"))
            return;

        var suffix = Guid.NewGuid().ToString("N");
        var parentTable = "SerenitySchemaIT_Parent_" + suffix;
        var childTable = "SerenitySchemaIT_Child_" + suffix;
        var rowIdTable = "SerenitySchemaIT_RowId_" + suffix;
        var withoutRowIdTable = "SerenitySchemaIT_WithoutRowId_" + suffix;
        var view = "SerenitySchemaIT_View_" + suffix;

        using var connection = SqlIntegrationConnections.CreateConnection("Sqlite");
        connection.Open();
        SqlIntegrationConnections.ExecuteSql(connection, "PRAGMA foreign_keys = ON;");
        SqlIntegrationConnections.ExecuteSql(connection, $"""
            CREATE TABLE [{parentTable}] (
                ParentId INTEGER PRIMARY KEY,
                Name TEXT
            );
            CREATE TABLE [{childTable}] (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ParentId INTEGER NOT NULL,
                Amount NUMERIC(10, 2) NOT NULL,
                FOREIGN KEY (ParentId) REFERENCES [{parentTable}](ParentId)
            );
            CREATE TABLE [{rowIdTable}] (Value TEXT);
            CREATE TABLE [{withoutRowIdTable}] (
                KeyA TEXT NOT NULL,
                KeyB TEXT NOT NULL,
                PRIMARY KEY (KeyA, KeyB)
            ) WITHOUT ROWID;
            CREATE VIEW [{view}] AS SELECT Id, ParentId FROM [{childTable}];
            """);

        var provider = new SqliteSchemaProvider();
        var fields = provider.GetFieldInfos(connection, null, childTable).ToList();
        Assert.Equal(["Id", "ParentId", "Amount"], fields.Select(x => x.FieldName));
        Assert.True(Assert.Single(fields, x => x.FieldName == "Id").IsIdentity);
        Assert.Contains(fields, x => x.FieldName == "Amount" && x.DataType == "NUMERIC(10, 2)");

        Assert.Equal(["Id"], provider.GetIdentityFields(connection, null, childTable).ToList());
        Assert.Equal(["Id"], provider.GetPrimaryKeyFields(connection, null, childTable).ToList());
        Assert.Equal(["ROWID"], provider.GetIdentityFields(connection, null, rowIdTable).ToList());
        Assert.Empty(provider.GetIdentityFields(connection, null, withoutRowIdTable));

        var foreignKey = Assert.Single(provider.GetForeignKeys(connection, null, childTable));
        Assert.Equal("ParentId", foreignKey.FKColumn);
        Assert.Equal(parentTable, foreignKey.PKTable);
        Assert.Equal("ParentId", foreignKey.PKColumn);

        var objects = provider.GetTableNames(connection)
            .Where(x => x.Table == parentTable || x.Table == childTable ||
                x.Table == rowIdTable || x.Table == withoutRowIdTable || x.Table == view)
            .ToDictionary(x => x.Table);
        Assert.Equal(5, objects.Count);
        Assert.All(objects.Where(x => x.Key != view), x => Assert.False(x.Value.IsView));
        Assert.True(objects[view].IsView);
    }
}
