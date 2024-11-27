using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serenity.Demo.Northwind.TerritoryRow>;
using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class TerritoryRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), ITerritoryRetrieveHandler
{
}