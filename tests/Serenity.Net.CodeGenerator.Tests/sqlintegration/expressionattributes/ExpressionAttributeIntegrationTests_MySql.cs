namespace Serenity.Data.Mapping;

[Trait("tag", "sqldb")]
public class ExpressionAttributeIntegrationTests_MySql : ExpressionAttributeIntegrationTests_Base
{
    protected override string Provider => "MySql";

    protected override ISqlDialect Dialect => MySqlDialect.Instance;
}
