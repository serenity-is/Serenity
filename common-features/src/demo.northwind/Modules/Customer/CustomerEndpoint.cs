using Serenity.Reporting;
using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind.Endpoints;

[Route("Services/Northwind/Customer/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class CustomerEndpoint : ServiceEndpoint
{
    [HttpPost, AuthorizeCreate(typeof(MyRow))]
    public Task<SaveResponse> Create(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] ICustomerSaveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.CreateAsync(uow, request, cancellationToken);
    }

    [HttpPost, AuthorizeUpdate(typeof(MyRow))]
    public Task<SaveResponse> Update(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] ICustomerSaveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.UpdateAsync(uow, request, cancellationToken);
    }

    [HttpPost, AuthorizeDelete(typeof(MyRow))]
    public Task<DeleteResponse> Delete(IUnitOfWork uow, DeleteRequest request,
        [FromServices] ICustomerDeleteHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.DeleteAsync(uow, request, cancellationToken);
    }

    public GetNextNumberResponse GetNextNumber(IDbConnection connection, GetNextNumberRequest request,
        [FromServices] ICustomerGetNextNumberHandler handler)
    {
        return handler.GetNextNumber(connection, request);
    }

    public Task<RetrieveResponse<MyRow>> Retrieve(IDbConnection connection, RetrieveRequest request,
        [FromServices] ICustomerRetrieveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.RetrieveAsync(connection, request, cancellationToken);
    }

    public Task<ListResponse<MyRow>> List(IDbConnection connection, ListRequest request,
        [FromServices] ICustomerListHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.ListAsync(connection, request, cancellationToken);
    }

    public async Task<FileContentResult> ListExcel(IDbConnection connection, ListRequest request,
        [FromServices] ICustomerListHandler handler,
        [FromServices] IExcelExporter exporter, CancellationToken cancellationToken = default)
    {
        var data = (await List(connection, request, handler, cancellationToken).ConfigureAwait(false)).Entities;
        var bytes = exporter.Export(data, typeof(Columns.CustomerColumns), request.ExportColumns);
        return ExcelContentResult.Create(bytes, "CustomerList_" +
            DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx");
    }
}
