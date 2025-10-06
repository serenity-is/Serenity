using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangListHandler : IListHandler<MyRow> { }

public class ProductLangListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), IProductLangListHandler
{
}