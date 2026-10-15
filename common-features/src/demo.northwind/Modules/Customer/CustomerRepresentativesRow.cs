namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("CustomerRepresentatives")]
[DisplayText("CustomerRepresentatives"), InstanceName("CustomerRepresentatives")]
[ReadPermission(PermissionKeys.Customer.View)]
[ModifyPermission(PermissionKeys.Customer.View)]
public sealed class CustomerRepresentativesRow : Row<CustomerRepresentativesRow.RowFields>, IIdRow
{
    [DisplayText("Representative Id"), Column("RepresentativeID"), Identity, IdProperty]
    public int? RepresentativeId { get => fields.RepresentativeId[this]; set => fields.RepresentativeId[this] = value; }

    [DisplayText("Customer Id"), Column("CustomerID"), Size(5), NotNull]
    public string? CustomerId { get => fields.CustomerId[this]; set => fields.CustomerId[this] = value; }

    [DisplayText("Employee Id"), Column("EmployeeID"), NotNull]
    public int? EmployeeId { get => fields.EmployeeId[this]; set => fields.EmployeeId[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field RepresentativeId = null!;
        public StringField CustomerId = null!;
        public Int32Field EmployeeId = null!;
    }
}