using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class RegionRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), IRegionRetrieveHandler
{
}