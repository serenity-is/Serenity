using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class ProductDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), IProductDeleteHandler
{
}