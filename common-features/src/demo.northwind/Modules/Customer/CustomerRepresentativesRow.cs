namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("CustomerRepresentatives")]
[DisplayName("CustomerRepresentatives"), InstanceName("CustomerRepresentatives")]
[ReadPermission(PermissionKeys.Customer.View)]
[ModifyPermission(PermissionKeys.Customer.View)]
public sealed class CustomerRepresentativesRow : Row<CustomerRepresentativesRow.RowFields>, IIdRow
{
    [DisplayName("Representative Id"), Column("RepresentativeID"), Identity, IdProperty]
    public int? RepresentativeId { get => fields.RepresentativeId[this]; set => fields.RepresentativeId[this] = value; }

    [DisplayName("Customer Id"), Column("CustomerID"), NotNull]
    public int? CustomerId { get => fields.CustomerId[this]; set => fields.CustomerId[this] = value; }

    [DisplayName("Employee Id"), Column("EmployeeID"), NotNull]
    public int? EmployeeId { get => fields.EmployeeId[this]; set => fields.EmployeeId[this] = value; }

    public CustomerRepresentativesRow()
    {
    }

    public CustomerRepresentativesRow(RowFields fields)
        : base(fields)
    {
    }

    public class RowFields : RowFieldsBase
    {
        public Int32Field RepresentativeId;
        public Int32Field CustomerId;
        public Int32Field EmployeeId;
    }
}