namespace Serenity.Data.Schema;

[Trait("tag", "sqldb")]
public partial class SchemaProviderIntegrationTests_Firebird
{
    [Fact]
    public void FirebirdSchemaProvider_Reads_Columns_Keys_ForeignKeys_And_Views()
    {
        var suffix = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
        var parentTable = "SIT_PARENT_" + suffix;
        var childTable = "SIT_CHILD_" + suffix;
        var view = "SIT_VIEW_" + suffix;
        var generator = "GEN_" + childTable + "_ID";
        var trigger = "SIT_TRG_" + suffix;
        var constraint = "SIT_FK_" + suffix;
        var createdParent = false;
        var createdChild = false;
        var createdGenerator = false;
        var createdTrigger = false;
        var createdView = false;

        using var connection = SqlIntegrationConnections.CreateConnection("Firebird");
        connection.Open();
        try
        {
            SqlIntegrationConnections.ExecuteSql(connection, $"""
                CREATE TABLE {parentTable} (
                    ID INTEGER NOT NULL PRIMARY KEY,
                    NAME VARCHAR(32)
                )
                """);
            createdParent = true;

            SqlIntegrationConnections.ExecuteSql(connection, $"""
                CREATE TABLE {childTable} (
                    ID INTEGER NOT NULL PRIMARY KEY,
                    PARENT_ID INTEGER NOT NULL,
                    AMOUNT DECIMAL(10,2) NOT NULL,
                    CONSTRAINT {constraint} FOREIGN KEY (PARENT_ID)
                        REFERENCES {parentTable}(ID)
                )
                """);
            createdChild = true;

            SqlIntegrationConnections.ExecuteSql(connection, $"CREATE GENERATOR {generator}");
            createdGenerator = true;

            SqlIntegrationConnections.ExecuteSql(connection, $"""
                CREATE TRIGGER {trigger} FOR {childTable}
                ACTIVE BEFORE INSERT POSITION 0
                AS
                BEGIN
                    IF (NEW.ID IS NULL) THEN
                        NEW.ID = GEN_ID({generator}, 1);
                END
                """);
            createdTrigger = true;

            SqlIntegrationConnections.ExecuteSql(connection, $"CREATE VIEW {view} (ID, PARENT_ID) AS SELECT ID, PARENT_ID FROM {childTable}");
            createdView = true;

            var provider = new FirebirdSchemaProvider();
            var fields = provider.GetFieldInfos(connection, null, childTable).ToList();
            Assert.Equal(["ID", "PARENT_ID", "AMOUNT"], fields.Select(x => x.FieldName));
            Assert.Contains(fields, x => x.FieldName == "AMOUNT" && x.DataType == "decimal" && x.Size == 10 && x.Scale == 2);
            Assert.Contains(fields, x => x.FieldName == "PARENT_ID" && !x.IsNullable);

            Assert.Equal(["ID"], provider.GetIdentityFields(connection, null, childTable).ToList());
            Assert.Equal(["ID"], provider.GetPrimaryKeyFields(connection, null, childTable).ToList());

            var foreignKey = Assert.Single(provider.GetForeignKeys(connection, null, childTable));
            Assert.Equal(constraint, foreignKey.FKName);
            Assert.Equal("PARENT_ID", foreignKey.FKColumn);
            Assert.Equal(parentTable, foreignKey.PKTable);
            Assert.Equal("ID", foreignKey.PKColumn);

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
            if (createdView)
                SqlIntegrationConnections.ExecuteSql(connection, $"DROP VIEW {view}");
            if (createdTrigger)
                SqlIntegrationConnections.ExecuteSql(connection, $"DROP TRIGGER {trigger}");
            if (createdChild)
                SqlIntegrationConnections.ExecuteSql(connection, $"DROP TABLE {childTable}");
            if (createdParent)
                SqlIntegrationConnections.ExecuteSql(connection, $"DROP TABLE {parentTable}");
            if (createdGenerator)
                SqlIntegrationConnections.ExecuteSql(connection, $"DROP GENERATOR {generator}");
        }
    }
}
