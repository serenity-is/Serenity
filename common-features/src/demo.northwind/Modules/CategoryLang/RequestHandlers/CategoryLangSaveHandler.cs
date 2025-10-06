using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangSaveHandler : ISaveHandler<MyRow> { }

public class CategoryLangSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), ICategoryLangSaveHandler
{
}