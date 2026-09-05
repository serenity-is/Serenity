using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class CategoryDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), ICategoryDeleteHandler
{
}