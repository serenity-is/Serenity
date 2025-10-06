using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierDeleteHandler : IDeleteHandler<MyRow> { }

public class SupplierDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), ISupplierDeleteHandler
{
}