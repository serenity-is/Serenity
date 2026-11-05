namespace Serenity.Data.Mapping;

[Trait("tag", "sqldb")]
public class ExpressionAttributeIntegrationTests_Postgres : ExpressionAttributeIntegrationTests_Base
{
    protected override string Provider => "Postgres";

    protected override ISqlDialect Dialect => PostgresDialect.Instance;
}
