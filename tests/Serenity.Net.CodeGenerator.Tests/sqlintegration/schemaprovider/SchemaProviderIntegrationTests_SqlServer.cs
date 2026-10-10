namespace Serenity.Data.Schema;

[Trait("tag", "sqldb")]
public partial class SchemaProviderIntegrationTests_SqlServer
{
    [Fact]
    public void SqlServerSchemaProvider_Reads_Tables_Keys_ForeignKeys_And_Views()
    {
        if (SqlIntegrationConnections.ShouldSkip("SqlServer"))
            return;

        var database = "SerenitySchemaIT_" + Guid.NewGuid().ToString("N");
        var quotedDatabase = "[" + database + "]";
        var parentTable = "Parent_" + Guid.NewGuid().ToString("N");
        var childTable = "Child_" + Guid.NewGuid().ToString("N");
        var view = "View_" + Guid.NewGuid().ToString("N");
        var constraint = "FK_" + Guid.NewGuid().ToString("N");
        var createdDatabase = false;

        using var connection = SqlIntegrationConnections.CreateConnection("SqlServer");
        connection.Open();
        try
        {
            SqlIntegrationConnections.ExecuteSql(connection, "CREATE DATABASE " + quotedDatabase);
            createdDatabase = true;
            connection.ChangeDatabase(database);

            SqlIntegrationConnections.ExecuteSql(connection, $"""
                CREATE TABLE dbo.[{parentTable}] (
                    ParentId int NOT NULL PRIMARY KEY,
                    Name nvarchar(32) NULL
                );
                CREATE TABLE dbo.[{childTable}] (
                    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    ParentId int NOT NULL,
                    Amount decimal(10,2) NOT NULL,
                    CONSTRAINT [{constraint}] FOREIGN KEY (ParentId)
                        REFERENCES dbo.[{parentTable}](ParentId)
                );
                """);
            SqlIntegrationConnections.ExecuteSql(connection, $"CREATE VIEW dbo.[{view}] AS SELECT Id, ParentId FROM dbo.[{childTable}];");

            var provider = new SqlServerSchemaProvider();
            var fields = provider.GetFieldInfos(connection, "dbo", childTable).ToList();
            Assert.Equal(["Id", "ParentId", "Amount"], fields.Select(x => x.FieldName));
            Assert.Contains(fields, x => x.FieldName == "Amount" && x.DataType == "decimal" && x.Size == 10 && x.Scale == 2);

            Assert.Equal(["Id"], provider.GetIdentityFields(connection, "dbo", childTable).ToList());
            Assert.Equal(["Id"], provider.GetPrimaryKeyFields(connection, "dbo", childTable).ToList());

            var foreignKey = Assert.Single(provider.GetForeignKeys(connection, "dbo", childTable));
            Assert.Equal("ParentId", foreignKey.FKColumn);
            Assert.Equal("dbo", foreignKey.PKSchema);
            Assert.Equal(parentTable, foreignKey.PKTable);
            Assert.Equal("ParentId", foreignKey.PKColumn);

            var objects = provider.GetTableNames(connection)
                .Where(x => x.Schema == "dbo" && (x.Table == parentTable || x.Table == childTable || x.Table == view))
                .ToDictionary(x => x.Table);
            Assert.Equal(3, objects.Count);
            Assert.False(objects[parentTable].IsView);
            Assert.False(objects[childTable].IsView);
            Assert.True(objects[view].IsView);
        }
        finally
        {
            if (createdDatabase)
            {
                connection.ChangeDatabase("master");
                SqlIntegrationConnections.ExecuteSql(connection, $"ALTER DATABASE {quotedDatabase} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {quotedDatabase};");
            }
        }
    }

}
