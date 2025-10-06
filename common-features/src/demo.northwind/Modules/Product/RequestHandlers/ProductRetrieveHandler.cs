using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductRetrieveHandler : IRetrieveHandler<MyRow> { }

public class ProductRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), IProductRetrieveHandler
{
}