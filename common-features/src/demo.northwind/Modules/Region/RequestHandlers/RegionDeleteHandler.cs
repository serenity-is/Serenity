using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;
using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class RegionDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), IRegionDeleteHandler
{
}