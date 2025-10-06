using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangListHandler : IListHandler<MyRow> { }

public class CategoryLangListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), ICategoryLangListHandler
{
}