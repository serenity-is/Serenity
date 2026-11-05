namespace Serenity.Data.Mapping;

[Trait("tag", "sqldb")]
public class ExpressionAttributeIntegrationTests_Oracle : ExpressionAttributeIntegrationTests_Base
{
    protected override string Provider => "Oracle";

    protected override ISqlDialect Dialect => OracleDialect.Instance;

    protected override string? From => "DUAL";
}
