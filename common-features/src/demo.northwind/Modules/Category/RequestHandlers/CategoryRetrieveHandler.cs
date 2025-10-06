using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryRetrieveHandler : IRetrieveHandler<MyRow> { }

public class CategoryRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), ICategoryRetrieveHandler
{
}