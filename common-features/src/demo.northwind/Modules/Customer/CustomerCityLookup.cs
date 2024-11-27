namespace Serenity.Demo.Northwind.Lookups;

[LookupScript, Module("Northwind")]
public class CustomerCityLookup : RowLookupScript<CustomerRow>
{
    public CustomerCityLookup(ISqlConnections sqlConnections)
        : base(sqlConnections)
    {
        IdField = TextField = CustomerRow.Fields.City.PropertyName;
    }

    protected override void PrepareQuery(SqlQuery query)
    {
        var fld = CustomerRow.Fields;
        query.Distinct(true)
            .Select(fld.Country)
            .Select(fld.City)
            .Where(
                new Criteria(fld.Country) != "" &
                new Criteria(fld.Country).IsNotNull() &
                new Criteria(fld.City) != "" &
                new Criteria(fld.City).IsNotNull());
    }

    protected override void ApplyOrder(SqlQuery query)
    {
    }
}