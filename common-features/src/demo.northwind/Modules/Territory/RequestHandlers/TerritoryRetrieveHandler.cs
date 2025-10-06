using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryRetrieveHandler : IRetrieveHandler<MyRow> { }

public class TerritoryRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), ITerritoryRetrieveHandler
{
}