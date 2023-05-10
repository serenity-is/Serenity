namespace Serenity.Services;

internal class DeleteHandlerProxy<TRow, TDeleteRequest, TDeleteResponse>
    : IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{
    private readonly IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse> handler;

    public DeleteHandlerProxy(IDefaultHandlerFactory factory)
    {
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));

        handler = (IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>) factory.CreateHandler<IDeleteRequestProcessor>(typeof(TRow));
    }

    public TDeleteResponse Delete(IUnitOfWork uow, TDeleteRequest request)
    {
        return handler.Delete(uow, request);
    }
}

internal class DeleteHandlerProxy<TRow>
    : DeleteHandlerProxy<TRow, DeleteRequest, DeleteResponse>, IDeleteHandler<TRow>
    where TRow : class, IRow, IIdRow, new()
{
    public DeleteHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
}