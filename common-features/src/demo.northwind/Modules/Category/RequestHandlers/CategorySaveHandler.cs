using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategorySaveHandler : ISaveHandler<MyRow> { }

public class CategorySaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), ICategorySaveHandler
{
}