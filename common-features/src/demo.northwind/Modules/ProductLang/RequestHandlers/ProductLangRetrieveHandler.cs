using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class ProductLangRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), IProductLangRetrieveHandler
{
}