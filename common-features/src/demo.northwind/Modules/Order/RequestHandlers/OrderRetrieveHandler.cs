using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serenity.Demo.Northwind.OrderRow>;
using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class OrderRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), IOrderRetrieveHandler
{
}