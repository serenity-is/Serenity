using System.Data.Common;

namespace Serenity.Services.SqlErrors;

[Collection(SqlIntegrationCollections.Firebird)]
[Trait("tag", "sqldb")]
public class SqlErrorIntegrationTests_Firebird : SqlErrorIntegrationTests_Base
{
    protected override string Provider => "Firebird";

    protected override ServerType ExpectedServerType => ServerType.Firebird;

    protected override void CreateTables(DbConnection connection)
    {
        Execute(connection, $"""
            CREATE TABLE {ParentTable} (
                ID INTEGER NOT NULL PRIMARY KEY,
                CODE VARCHAR(32) NOT NULL,
                REQUIREDCOL VARCHAR(32) NOT NULL,
                CONSTRAINT UQ_{ParentTable} UNIQUE (CODE)
            )
            """);
        Execute(connection, $"""
            CREATE TABLE {ChildTable} (
                ID INTEGER NOT NULL PRIMARY KEY,
                PARENTID INTEGER NOT NULL,
                CONSTRAINT FK_{ChildTable} FOREIGN KEY (PARENTID)
                    REFERENCES {ParentTable}(ID)
            )
            """);
    }

    protected override void DropTables(DbConnection connection)
    {
        TryDrop(connection, $"DROP TABLE {ChildTable}");
        TryDrop(connection, $"DROP TABLE {ParentTable}");
    }

    protected override string InsertParent(int id, string code, string? required) =>
        $"INSERT INTO {ParentTable} (ID, CODE, REQUIREDCOL) VALUES ({id}, '{code}', {(required is null ? "NULL" : "'" + required + "'")})";

    protected override string InsertChild(int id, int parentId) =>
        $"INSERT INTO {ChildTable} (ID, PARENTID) VALUES ({id}, {parentId})";

    protected override string DeleteParent(int id) =>
        $"DELETE FROM {ParentTable} WHERE ID = {id}";

    private static void TryDrop(DbConnection connection, string sql)
    {
        try
        {
            Execute(connection, sql);
        }
        catch
        {
            // table might not have been created if setup failed
        }
    }
}
