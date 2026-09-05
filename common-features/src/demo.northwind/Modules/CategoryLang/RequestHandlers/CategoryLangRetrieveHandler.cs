using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class CategoryLangRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), ICategoryLangRetrieveHandler
{
}