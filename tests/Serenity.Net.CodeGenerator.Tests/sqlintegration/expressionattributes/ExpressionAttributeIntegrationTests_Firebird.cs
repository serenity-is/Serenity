namespace Serenity.Data.Mapping;

[Collection(SqlIntegrationCollections.Firebird)]
[Trait("tag", "sqldb")]
public class ExpressionAttributeIntegrationTests_Firebird : ExpressionAttributeIntegrationTests_Base
{
    protected override string Provider => "Firebird";

    protected override ISqlDialect Dialect => FirebirdDialect.Instance;

    protected override string? From => "RDB$DATABASE";
}
