using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductSaveHandler : ISaveHandler<MyRow> { }

public class ProductSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), IProductSaveHandler
{
}