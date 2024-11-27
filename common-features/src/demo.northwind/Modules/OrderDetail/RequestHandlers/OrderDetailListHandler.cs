using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serenity.Demo.Northwind.OrderDetailRow>;
using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class OrderDetailListHandler(IRequestContext context) :
    ListRequestHandler<MyRow, MyRequest, MyResponse>(context), IOrderDetailListHandler
{
}