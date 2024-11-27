using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;
using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class TerritoryDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), ITerritoryDeleteHandler
{
}