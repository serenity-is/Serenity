using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind;

public interface IProductLangSaveHandler : ISaveHandlerAsync<MyRow> { }

public class ProductLangSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), IProductLangSaveHandler
{
}