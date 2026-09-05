using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class SupplierRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), ISupplierRetrieveHandler
{
}