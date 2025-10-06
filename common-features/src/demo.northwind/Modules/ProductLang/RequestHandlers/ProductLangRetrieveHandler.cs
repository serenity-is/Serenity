using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangRetrieveHandler : IRetrieveHandler<MyRow> { }

public class ProductLangRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), IProductLangRetrieveHandler
{
}