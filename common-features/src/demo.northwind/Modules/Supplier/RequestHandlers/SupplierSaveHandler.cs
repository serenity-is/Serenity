using MyRow = Serenity.Demo.Northwind.SupplierRow;

namespace Serenity.Demo.Northwind;

public interface ISupplierSaveHandler : ISaveHandler<MyRow> { }

public class SupplierSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), ISupplierSaveHandler
{
}