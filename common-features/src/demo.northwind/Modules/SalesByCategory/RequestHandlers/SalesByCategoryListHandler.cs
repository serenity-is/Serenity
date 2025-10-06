using MyRow = Serenity.Demo.Northwind.SalesByCategoryRow;

namespace Serenity.Demo.Northwind;

public interface ISalesByCategoryListHandler : IListHandler<MyRow> { }

public class SalesByCategoryListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), ISalesByCategoryListHandler
{
}