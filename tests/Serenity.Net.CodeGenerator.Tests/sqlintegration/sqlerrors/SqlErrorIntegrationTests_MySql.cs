using System.Data.Common;

namespace Serenity.Services.SqlErrors;

[Trait("tag", "sqldb")]
public class SqlErrorIntegrationTests_MySql : SqlErrorIntegrationTests_Base
{
    protected override string Provider => "MySql";

    protected override ServerType ExpectedServerType => ServerType.MySql;

    protected override void CreateTables(DbConnection connection)
    {
        Execute(connection, $"""
            CREATE TABLE `{ParentTable}` (
                Id int NOT NULL PRIMARY KEY,
                Code varchar(32) NOT NULL UNIQUE,
                RequiredCol varchar(32) NOT NULL
            ) ENGINE=InnoDB
            """);
        Execute(connection, $"""
            CREATE TABLE `{ChildTable}` (
                Id int NOT NULL PRIMARY KEY,
                ParentId int NOT NULL,
                CONSTRAINT `FK_{ChildTable}` FOREIGN KEY (ParentId)
                    REFERENCES `{ParentTable}`(Id)
            ) ENGINE=InnoDB
            """);
    }

    protected override void DropTables(DbConnection connection)
    {
        Execute(connection, $"DROP TABLE IF EXISTS `{ChildTable}`");
        Execute(connection, $"DROP TABLE IF EXISTS `{ParentTable}`");
    }

    protected override string InsertParent(int id, string code, string? required) =>
        $"INSERT INTO `{ParentTable}` (Id, Code, RequiredCol) VALUES ({id}, '{code}', {(required is null ? "NULL" : "'" + required + "'")})";

    protected override string InsertChild(int id, int parentId) =>
        $"INSERT INTO `{ChildTable}` (Id, ParentId) VALUES ({id}, {parentId})";

    protected override string DeleteParent(int id) =>
        $"DELETE FROM `{ParentTable}` WHERE Id = {id}";
}
