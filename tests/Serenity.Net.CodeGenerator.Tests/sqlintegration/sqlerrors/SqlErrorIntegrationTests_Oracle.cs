using System.Data.Common;

namespace Serenity.Services.SqlErrors;

[Trait("tag", "sqldb")]
public class SqlErrorIntegrationTests_Oracle : SqlErrorIntegrationTests_Base
{
    protected override string Provider => "Oracle";

    protected override ServerType ExpectedServerType => ServerType.Oracle;

    protected override void CreateTables(DbConnection connection)
    {
        Execute(connection, $"""
            CREATE TABLE {ParentTable} (
                ID NUMBER(10) NOT NULL PRIMARY KEY,
                CODE VARCHAR2(32) NOT NULL,
                REQUIREDCOL VARCHAR2(32) NOT NULL,
                CONSTRAINT UQ_{ParentTable} UNIQUE (CODE)
            )
            """);
        Execute(connection, $"""
            CREATE TABLE {ChildTable} (
                ID NUMBER(10) NOT NULL PRIMARY KEY,
                PARENTID NUMBER(10) NOT NULL,
                CONSTRAINT FK_{ChildTable} FOREIGN KEY (PARENTID)
                    REFERENCES {ParentTable}(ID)
            )
            """);
    }

    protected override void DropTables(DbConnection connection)
    {
        TryDrop(connection, $"DROP TABLE {ChildTable} CASCADE CONSTRAINTS PURGE");
        TryDrop(connection, $"DROP TABLE {ParentTable} CASCADE CONSTRAINTS PURGE");
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
