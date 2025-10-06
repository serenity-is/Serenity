using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierListHandler : IListHandler<MyRow> { }

public class SupplierListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), ISupplierListHandler
{
}