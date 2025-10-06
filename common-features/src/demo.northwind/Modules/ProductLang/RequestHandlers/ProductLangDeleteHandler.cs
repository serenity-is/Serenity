using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangDeleteHandler : IDeleteHandler<MyRow> { }

public class ProductLangDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), IProductLangDeleteHandler
{
}