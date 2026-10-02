
namespace Serenity.Data.Schema;

[Trait("tag", "sqldb")]
public partial class SchemaProviderIntegrationTests_MySql
{
    [Fact]
    public void MySqlSchemaProvider_Reads_Columns_Keys_ForeignKeys_And_Views()
    {
        var database = "serenity_schema_it_" + Guid.NewGuid().ToString("N");
        var parentTable = "parent_" + Guid.NewGuid().ToString("N");
        var childTable = "child_" + Guid.NewGuid().ToString("N");
        var view = "view_" + Guid.NewGuid().ToString("N");
        var constraint = "fk_" + Guid.NewGuid().ToString("N");
        var createdDatabase = false;

        using var connection = SqlIntegrationConnections.CreateConnection("MySql");
        connection.Open();
        try
        {
            SqlIntegrationConnections.ExecuteSql(connection, "CREATE DATABASE `" + database + "`");
            createdDatabase = true;
            connection.ChangeDatabase(database);

            SqlIntegrationConnections.ExecuteSql(connection, $"""
                CREATE TABLE `{parentTable}` (
                    ParentId int NOT NULL PRIMARY KEY,
                    Name varchar(32) NULL
                ) ENGINE=InnoDB
                """);
            SqlIntegrationConnections.ExecuteSql(connection, $"""
                CREATE TABLE `{childTable}` (
                    Id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    ParentId int NOT NULL,
                    Amount decimal(10,2) NOT NULL,
                    CONSTRAINT `{constraint}` FOREIGN KEY (ParentId)
                        REFERENCES `{parentTable}`(ParentId)
                ) ENGINE=InnoDB
                """);
            SqlIntegrationConnections.ExecuteSql(connection, $"CREATE VIEW `{view}` AS SELECT Id, ParentId FROM `{childTable}`");

            var provider = new MySqlSchemaProvider();
            var fields = provider.GetFieldInfos(connection, database, childTable).ToList();
            Assert.Equal(["Id", "ParentId", "Amount"], fields.Select(x => x.FieldName));
            Assert.True(Assert.Single(fields, x => x.FieldName == "Id").IsIdentity);
            Assert.Contains(fields, x => x.FieldName == "Amount" && x.DataType == "decimal" && x.Size == 10 && x.Scale == 2);

            Assert.Equal(["Id"], provider.GetIdentityFields(connection, database, childTable).ToList());
            Assert.Equal(["Id"], provider.GetPrimaryKeyFields(connection, database, childTable).ToList());

            var foreignKey = Assert.Single(provider.GetForeignKeys(connection, database, childTable));
            Assert.Equal("ParentId", foreignKey.FKColumn);
            Assert.Null(foreignKey.PKSchema);
            Assert.Equal(parentTable, foreignKey.PKTable);
            Assert.Equal("ParentId", foreignKey.PKColumn);

            var objects = provider.GetTableNames(connection)
                .Where(x => x.Table == parentTable || x.Table == childTable || x.Table == view)
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
                connection.ChangeDatabase("mysql");
                SqlIntegrationConnections.ExecuteSql(connection, "DROP DATABASE `" + database + "`");
            }
        }
    }
}
