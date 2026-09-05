using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategorySaveHandler : ISaveHandlerAsync<MyRow> { }

public class CategorySaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), ICategorySaveHandler
{
}