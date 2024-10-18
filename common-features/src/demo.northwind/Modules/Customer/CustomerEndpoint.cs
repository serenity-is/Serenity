using Microsoft.AspNetCore.Mvc;
using Serenity.Reporting;
using System.Data;
using MyRow = Serenity.Demo.Northwind.CustomerRow;

namespace Serenity.Demo.Northwind.Endpoints;

[Route("Services/Serenity.Demo.Northwind/Customer/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class CustomerEndpoint : ServiceEndpoint
{
    [HttpPost, AuthorizeCreate(typeof(MyRow))]
    public SaveResponse Create(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] ICustomerSaveHandler handler)
    {
        return handler.Create(uow, request);
    }

    [HttpPost, AuthorizeUpdate(typeof(MyRow))]
    public SaveResponse Update(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] ICustomerSaveHandler handler)
    {
        return handler.Update(uow, request);
    }

    [HttpPost, AuthorizeDelete(typeof(MyRow))]
    public DeleteResponse Delete(IUnitOfWork uow, DeleteRequest request,
        [FromServices] ICustomerDeleteHandler handler)
    {
        return handler.Delete(uow, request);
    }

    public GetNextNumberResponse GetNextNumber(IDbConnection connection, GetNextNumberRequest request,
        [FromServices] ICustomerGetNextNumberHandler handler)
    {
        return handler.GetNextNumber(connection, request);
    }

    public RetrieveResponse<MyRow> Retrieve(IDbConnection connection, RetrieveRequest request,
        [FromServices] ICustomerRetrieveHandler handler)
    {
        return handler.Retrieve(connection, request);
    }

    public ListResponse<MyRow> List(IDbConnection connection, ListRequest request,
        [FromServices] ICustomerListHandler handler)
    {
        return handler.List(connection, request);
    }

    public FileContentResult ListExcel(IDbConnection connection, ListRequest request,
        [FromServices] ICustomerListHandler handler,
        [FromServices] IExcelExporter exporter)
    {
        var data = List(connection, request, handler).Entities;
        var bytes = exporter.Export(data, typeof(Columns.CustomerColumns), request.ExportColumns);
        return ExcelContentResult.Create(bytes, "CustomerList_" +
            DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".xlsx");
    }
}