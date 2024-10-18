using Microsoft.AspNetCore.Mvc;
using System.Data;
using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind.Endpoints;

[Route("Services/Serenity.Demo.Northwind/OrderDetail/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class OrderDetailEndpoint : ServiceEndpoint
{
    public RetrieveResponse<MyRow> Retrieve(IDbConnection connection, RetrieveRequest request,
        [FromServices] IOrderDetailRetrieveHandler handler)
    {
        return handler.Retrieve(connection, request);
    }

    public ListResponse<MyRow> List(IDbConnection connection, ListRequest request,
        [FromServices] IOrderDetailListHandler handler)
    {
        return handler.List(connection, request);
    }
}
