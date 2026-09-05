using MyRow = Serenity.Demo.Northwind.ProductLangRow;

namespace Serenity.Demo.Northwind.Endpoints;

[Route("Services/Northwind/ProductLang/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class ProductLangEndpoint : ServiceEndpoint
{
    [HttpPost, AuthorizeCreate(typeof(MyRow))]
    public Task<SaveResponse> Create(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] IProductLangSaveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.CreateAsync(uow, request, cancellationToken);
    }

    [HttpPost, AuthorizeUpdate(typeof(MyRow))]
    public Task<SaveResponse> Update(IUnitOfWork uow, SaveRequest<MyRow> request,
        [FromServices] IProductLangSaveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.UpdateAsync(uow, request, cancellationToken);
    }

    [HttpPost, AuthorizeDelete(typeof(MyRow))]
    public Task<DeleteResponse> Delete(IUnitOfWork uow, DeleteRequest request,
        [FromServices] IProductLangDeleteHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.DeleteAsync(uow, request, cancellationToken);
    }

    public Task<RetrieveResponse<MyRow>> Retrieve(IDbConnection connection, RetrieveRequest request,
        [FromServices] IProductLangRetrieveHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.RetrieveAsync(connection, request, cancellationToken);
    }

    public Task<ListResponse<MyRow>> List(IDbConnection connection, ListRequest request,
        [FromServices] IProductLangListHandler handler, CancellationToken cancellationToken = default)
    {
        return handler.ListAsync(connection, request, cancellationToken);
    }
}
