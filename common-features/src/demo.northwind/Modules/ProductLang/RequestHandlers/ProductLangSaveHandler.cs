using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangSaveHandler : ISaveHandler<MyRow> { }

public class ProductLangSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), IProductLangSaveHandler
{
}