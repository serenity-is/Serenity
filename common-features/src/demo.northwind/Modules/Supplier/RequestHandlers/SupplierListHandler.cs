using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierListHandler : IListHandlerAsync<MyRow> { }

public class SupplierListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), ISupplierListHandler
{
}