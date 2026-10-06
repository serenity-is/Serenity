using System.Data.Common;

namespace Serenity.Services.SqlErrors;

[Trait("tag", "sqldb")]
public class SqlErrorIntegrationTests_SqlServer : SqlErrorIntegrationTests_Base
{
    protected override string Provider => "SqlServer";

    protected override ServerType ExpectedServerType => ServerType.SqlServer;

    protected override void CreateTables(DbConnection connection)
    {
        Execute(connection, $"""
            CREATE TABLE dbo.{ParentTable} (
                Id int NOT NULL PRIMARY KEY,
                Code nvarchar(32) NOT NULL UNIQUE,
                RequiredCol nvarchar(32) NOT NULL
            )
            """);
        Execute(connection, $"""
            CREATE TABLE dbo.{ChildTable} (
                Id int NOT NULL PRIMARY KEY,
                ParentId int NOT NULL,
                CONSTRAINT FK_{ChildTable} FOREIGN KEY (ParentId)
                    REFERENCES dbo.{ParentTable}(Id)
            )
            """);
    }

    protected override void DropTables(DbConnection connection)
    {
        Execute(connection, $"DROP TABLE IF EXISTS dbo.{ChildTable}");
        Execute(connection, $"DROP TABLE IF EXISTS dbo.{ParentTable}");
    }

    protected override string InsertParent(int id, string code, string? required) =>
        $"INSERT INTO dbo.{ParentTable} (Id, Code, RequiredCol) VALUES ({id}, '{code}', {(required is null ? "NULL" : "'" + required + "'")})";

    protected override string InsertChild(int id, int parentId) =>
        $"INSERT INTO dbo.{ChildTable} (Id, ParentId) VALUES ({id}, {parentId})";

    protected override string DeleteParent(int id) =>
        $"DELETE FROM dbo.{ParentTable} WHERE Id = {id}";
}
