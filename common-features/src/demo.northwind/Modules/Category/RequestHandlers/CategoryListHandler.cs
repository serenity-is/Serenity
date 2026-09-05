using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryListHandler : IListHandlerAsync<MyRow> { }

public class CategoryListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), ICategoryListHandler
{
}