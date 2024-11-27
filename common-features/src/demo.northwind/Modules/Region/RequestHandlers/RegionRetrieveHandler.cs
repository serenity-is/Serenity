using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serenity.Demo.Northwind.RegionRow>;
using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class RegionRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), IRegionRetrieveHandler
{
}