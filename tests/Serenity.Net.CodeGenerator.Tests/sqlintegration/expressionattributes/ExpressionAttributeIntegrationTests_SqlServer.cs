namespace Serenity.Data.Mapping;

[Trait("tag", "sqldb")]
public class ExpressionAttributeIntegrationTests_SqlServer : ExpressionAttributeIntegrationTests_Base
{
    protected override string Provider => "SqlServer";

    protected override ISqlDialect Dialect => SqlServer2012Dialect.Instance;
}
