using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangSaveHandler : ISaveHandlerAsync<MyRow> { }

public class CategoryLangSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), ICategoryLangSaveHandler
{
}