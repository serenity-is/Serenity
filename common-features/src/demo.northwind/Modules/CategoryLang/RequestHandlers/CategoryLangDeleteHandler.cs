using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class CategoryLangDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), ICategoryLangDeleteHandler
{
}