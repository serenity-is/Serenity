using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class SupplierDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), ISupplierDeleteHandler
{
}