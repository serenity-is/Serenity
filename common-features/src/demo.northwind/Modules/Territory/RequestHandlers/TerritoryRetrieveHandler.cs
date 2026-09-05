using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class TerritoryRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), ITerritoryRetrieveHandler
{
}