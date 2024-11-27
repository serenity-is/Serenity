namespace Serenity.Demo.Northwind.Lookups;

[LookupScript, Module("Northwind")]
public class OrderShipCityLookup : RowLookupScript<OrderRow>
{
    public OrderShipCityLookup(ISqlConnections sqlConnections)
        : base(sqlConnections)
    {
        IdField = TextField = OrderRow.Fields.ShipCity.PropertyName;
    }

    protected override void PrepareQuery(SqlQuery query)
    {
        var fld = OrderRow.Fields;
        query.Distinct(true)
            .Select(fld.ShipCountry)
            .Select(fld.ShipCity)
            .Where(
                fld.ShipCountry != "" &
                fld.ShipCountry.IsNotNull() &
                fld.ShipCity != "" &
                fld.ShipCity.IsNotNull());
    }

    protected override void ApplyOrder(SqlQuery query)
    {
    }
}