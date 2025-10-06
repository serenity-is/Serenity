using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryDeleteHandler : IDeleteHandler<MyRow> { }

public class CategoryDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), ICategoryDeleteHandler
{
}