using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductListHandler : IListHandlerAsync<MyRow> { }

public class ProductListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), IProductListHandler
{
}