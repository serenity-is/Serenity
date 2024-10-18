namespace Serenity.Demo.Northwind.Lookups;

[LookupScript, Module("Northwind")]
public class SupplierCountryLookup : RowLookupScript<SupplierRow>
{
    public SupplierCountryLookup(ISqlConnections sqlConnections)
        : base(sqlConnections)
    {
        IdField = TextField = "Country";
    }
    
    protected override void PrepareQuery(SqlQuery query)
    {
        var fld = SupplierRow.Fields;
        query.Distinct(true)
            .Select(fld.Country)
            .Where(
                new Criteria(fld.Country) != "" &
                new Criteria(fld.Country).IsNotNull());
    }

    protected override void ApplyOrder(SqlQuery query)
    {
    }
}