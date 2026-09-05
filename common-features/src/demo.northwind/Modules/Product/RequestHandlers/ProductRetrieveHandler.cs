using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class ProductRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), IProductRetrieveHandler
{
}