using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierSaveHandler : ISaveHandlerAsync<MyRow> { }

public class SupplierSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), ISupplierSaveHandler
{
}