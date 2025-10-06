using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierRetrieveHandler : IRetrieveHandler<MyRow> { }

public class SupplierRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), ISupplierRetrieveHandler
{
}