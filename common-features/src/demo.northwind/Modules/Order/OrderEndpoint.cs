using Serenity.Reporting;
using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind.Endpoints;

[Route("Services/Northwind/Order/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class OrderEndpoint : ServiceEndpoint
{
    [HttpPost, AuthorizeCreate(typeof(MyRow))]
    public Task<SaveResponse> Create(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] IOrderSaveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.CreateAsync(uow, request, cancellationToken);
    }

    [HttpPost, AuthorizeUpdate(typeof(MyRow))]
    public Task<SaveResponse> Update(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] IOrderSaveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.UpdateAsync(uow, request, cancellationToken);
    }

    [HttpPost, AuthorizeDelete(typeof(MyRow))]
    public Task<DeleteResponse> Delete(IUnitOfWork uow, DeleteRequest request,
        [FromServices] IOrderDeleteHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.DeleteAsync(uow, request, cancellationToken);
    }

    public Task<RetrieveResponse<MyRow>> Retrieve(IDbConnection connection, RetrieveRequest request,
        [FromServices] IOrderRetrieveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.RetrieveAsync(connection, request, cancellationToken);
    }

    public Task<ListResponse<MyRow>> List(IDbConnection connection, OrderListRequest request,
        [FromServices] IOrderListHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.ListAsync(connection, request, cancellationToken);
    }

    public async Task<FileContentResult> ListExcel(IDbConnection connection, OrderListRequest request,
        [FromServices] IExcelExporter exporter,
        [FromServices] IOrderListHandler handler, CancellationToken cancellationToken = default)
    {
        var data = (await List(connection, request, handler, cancellationToken).ConfigureAwait(false)).Entities;
        var bytes = exporter.Export(data, typeof(Columns.OrderColumns), request.ExportColumns);
        return ExcelContentResult.Create(bytes, "OrderList_" +
            DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx");
    }
}
