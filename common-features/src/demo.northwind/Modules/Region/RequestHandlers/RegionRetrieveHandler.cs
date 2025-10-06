using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionRetrieveHandler : IRetrieveHandler<MyRow> { }

public class RegionRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), IRegionRetrieveHandler
{
}