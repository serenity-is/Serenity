using MyRow = Serenity.Demo.Northwind.CategoryRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class CategoryRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), ICategoryRetrieveHandler
{
}