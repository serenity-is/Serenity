namespace Serenity.Demo.Northwind.Lookups;

[LookupScript, Module("Northwind")]
public class CustomerCountryLookup : RowLookupScript<CustomerRow>
{
    public CustomerCountryLookup(ISqlConnections sqlConnections)
        : base(sqlConnections)
    {
        IdField = TextField = "Country";
    }

    protected override void PrepareQuery(SqlQuery query)
    {
        var fld = CustomerRow.Fields;
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