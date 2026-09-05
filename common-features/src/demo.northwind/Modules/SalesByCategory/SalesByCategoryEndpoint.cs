using MyRow = Serenity.Demo.Northwind.SalesByCategoryRow;

namespace Serenity.Demo.Northwind.Endpoints;

[Route("Services/Northwind/SalesByCategory/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class SalesByCategoryEndpoint : ServiceEndpoint
{
    public Task<ListResponse<MyRow>> List(IDbConnection connection, ListRequest request,
        [FromServices] ISalesByCategoryListHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.ListAsync(connection, request, cancellationToken);
    }
}
