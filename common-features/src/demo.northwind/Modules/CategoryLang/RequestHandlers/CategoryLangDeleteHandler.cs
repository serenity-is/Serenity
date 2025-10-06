using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangDeleteHandler : IDeleteHandler<MyRow> { }

public class CategoryLangDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), ICategoryLangDeleteHandler
{
}