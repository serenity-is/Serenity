using MyRow = Serenity.Demo.Northwind.CategoryLangRow;

namespace Serenity.Demo.Northwind;

public interface ICategoryLangRetrieveHandler : IRetrieveHandler<MyRow> { }

public class CategoryLangRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), ICategoryLangRetrieveHandler
{
}