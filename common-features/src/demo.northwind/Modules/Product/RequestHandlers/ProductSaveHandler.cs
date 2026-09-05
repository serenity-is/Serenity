using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductSaveHandler : ISaveHandlerAsync<MyRow> { }

public class ProductSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), IProductSaveHandler
{
}