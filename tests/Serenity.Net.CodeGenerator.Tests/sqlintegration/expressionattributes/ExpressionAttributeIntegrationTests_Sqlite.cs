namespace Serenity.Data.Mapping;

[Trait("tag", "sqldb")]
public class ExpressionAttributeIntegrationTests_Sqlite : ExpressionAttributeIntegrationTests_Base
{
    protected override string Provider => "Sqlite";

    protected override ISqlDialect Dialect => SqliteDialect.Instance;
}
