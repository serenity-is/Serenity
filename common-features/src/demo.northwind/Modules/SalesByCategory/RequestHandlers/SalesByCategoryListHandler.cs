using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serenity.Demo.Northwind.SalesByCategoryRow>;
using MyRow = Serenity.Demo.Northwind.SalesByCategoryRow;

namespace Serenity.Demo.Northwind;

public interface ISalesByCategoryListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class SalesByCategoryListHandler(IRequestContext context) :
    ListRequestHandler<MyRow, MyRequest, MyResponse>(context), ISalesByCategoryListHandler
{
}