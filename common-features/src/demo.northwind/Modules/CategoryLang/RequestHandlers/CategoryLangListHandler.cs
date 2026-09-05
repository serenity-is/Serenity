using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangListHandler : IListHandlerAsync<MyRow> { }

public class CategoryLangListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), ICategoryLangListHandler
{
}