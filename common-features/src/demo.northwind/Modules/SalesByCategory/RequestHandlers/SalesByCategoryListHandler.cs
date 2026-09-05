using MyRow = Serenity.Demo.Northwind.SalesByCategoryRow;

namespace Serenity.Demo.Northwind;

public interface ISalesByCategoryListHandler : IListHandlerAsync<MyRow> { }

public class SalesByCategoryListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), ISalesByCategoryListHandler
{
}