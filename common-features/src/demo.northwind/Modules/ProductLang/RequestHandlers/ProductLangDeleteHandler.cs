using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class ProductLangDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), IProductLangDeleteHandler
{
}