using MyRequest = Serenity.Services.SaveRequest<Serenity.Demo.Northwind.RegionRow>;
using MyResponse = Serenity.Services.SaveResponse;
using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class RegionSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), IRegionSaveHandler
{
}