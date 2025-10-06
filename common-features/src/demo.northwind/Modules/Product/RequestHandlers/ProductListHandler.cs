using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductListHandler : IListHandler<MyRow> { }

public class ProductListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), IProductListHandler
{
}