using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryListHandler : IListHandler<MyRow> { }

public class CategoryListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), ICategoryListHandler
{
}