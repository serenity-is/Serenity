using MyRow = Serenity.Demo.Northwind.ProductRow;

namespace Serenity.Demo.Northwind;

public interface IProductDeleteHandler : IDeleteHandler<MyRow> { }

public class ProductDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), IProductDeleteHandler
{
}