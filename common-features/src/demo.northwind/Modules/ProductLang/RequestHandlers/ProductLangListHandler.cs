using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangListHandler : IListHandlerAsync<MyRow> { }

public class ProductLangListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), IProductLangListHandler
{
}