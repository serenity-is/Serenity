using System.Threading;
using System.Threading.Tasks;

namespace Serenity.Services
{
    internal class RetrieveHandlerProxy<TRow, TRetrieveRequest, TRetrieveResponse>
        : IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>
        where TRow : class, IRow, IIdRow, new()
        where TRetrieveRequest : RetrieveRequest
        where TRetrieveResponse : RetrieveResponse<TRow>, new()
    {
        private readonly IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse> handler;

        public RetrieveHandlerProxy(IDefaultHandlerFactory factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            handler = (IRetrieveHandler<TRow, TRetrieveRequest, TRetrieveResponse>) factory.CreateHandler<IRetrieveRequestProcessor>(typeof(TRow));
        }

        public TRetrieveResponse Retrieve(IDbConnection connection, TRetrieveRequest request)
        {
            return handler.Retrieve(connection, request);
        }

        public Task<TRetrieveResponse> RetrieveAsync(IDbConnection connection, TRetrieveRequest request, CancellationToken cancellationToken)
        {
            return handler.RetrieveAsync(connection, request, cancellationToken);
        }
    }

    internal class RetrieveHandlerProxy<TRow>
        : RetrieveHandlerProxy<TRow, RetrieveRequest, RetrieveResponse<TRow>>, IRetrieveHandler<TRow>
        where TRow : class, IRow, IIdRow, new()
    {
        public RetrieveHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
    }
}