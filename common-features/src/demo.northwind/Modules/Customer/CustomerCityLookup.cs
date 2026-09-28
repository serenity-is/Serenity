namespace Serenity.Demo.Northwind.Lookups;

[LookupScript, NorthwindModule]
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
                fld.Country != "" &
                fld.Country.IsNotNull() &
                fld.City != "" &
                fld.City.IsNotNull());
    }

    protected override void ApplyOrder(SqlQuery query)
    {
    }
}