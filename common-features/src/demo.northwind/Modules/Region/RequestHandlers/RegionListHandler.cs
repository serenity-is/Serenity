using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serenity.Demo.Northwind.RegionRow>;
using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class RegionListHandler(IRequestContext context) :
    ListRequestHandler<MyRow, MyRequest, MyResponse>(context), IRegionListHandler
{
}