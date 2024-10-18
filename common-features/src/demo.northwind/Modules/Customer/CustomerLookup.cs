namespace Serenity.Demo.Northwind.Lookups;

[LookupScript, Module("Northwind")]
public class CustomerLookup : RowLookupScript<CustomerRow>
{
    public CustomerLookup(ISqlConnections sqlConnections)
        : base(sqlConnections)
    {
        IdField = CustomerRow.Fields.CustomerID.PropertyName;
        TextField = CustomerRow.Fields.CompanyName.PropertyName;
    }
}