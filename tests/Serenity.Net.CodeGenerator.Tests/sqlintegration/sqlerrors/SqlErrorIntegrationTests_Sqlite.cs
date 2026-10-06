using System.Data.Common;

namespace Serenity.Services.SqlErrors;

public class SqlErrorIntegrationTests_Sqlite : SqlErrorIntegrationTests_Base
{
    protected override string Provider => "Sqlite";

    protected override ServerType ExpectedServerType => ServerType.Sqlite;

    protected override void CreateTables(DbConnection connection)
    {
        Execute(connection, "PRAGMA foreign_keys = ON");
        Execute(connection, $"""
            CREATE TABLE {ParentTable} (
                Id INTEGER NOT NULL PRIMARY KEY,
                Code TEXT NOT NULL UNIQUE,
                RequiredCol TEXT NOT NULL
            )
            """);
        Execute(connection, $"""
            CREATE TABLE {ChildTable} (
                Id INTEGER NOT NULL PRIMARY KEY,
                ParentId INTEGER NOT NULL,
                FOREIGN KEY (ParentId) REFERENCES {ParentTable}(Id)
            )
            """);
    }

    protected override void DropTables(DbConnection connection)
    {
        Execute(connection, $"DROP TABLE IF EXISTS {ChildTable}");
        Execute(connection, $"DROP TABLE IF EXISTS {ParentTable}");
    }

    protected override string InsertParent(int id, string code, string? required) =>
        $"INSERT INTO {ParentTable} (Id, Code, RequiredCol) VALUES ({id}, '{code}', {(required is null ? "NULL" : "'" + required + "'")})";

    protected override string InsertChild(int id, int parentId) =>
        $"INSERT INTO {ChildTable} (Id, ParentId) VALUES ({id}, {parentId})";

    protected override string DeleteParent(int id) =>
        $"DELETE FROM {ParentTable} WHERE Id = {id}";
}
