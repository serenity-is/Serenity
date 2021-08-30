using Serenity.Data;
using System;

namespace Serenity.Services
{
    internal class CreateHandlerProxy<TRow, TSaveRequest, TSaveResponse>
        : ICreateHandler<TRow, TSaveRequest, TSaveResponse>
        where TRow : class, IRow, IIdRow, new()
        where TSaveResponse : SaveResponse, new()
        where TSaveRequest : SaveRequest<TRow>, new()
    {
        private readonly ICreateHandler<TRow, TSaveRequest, TSaveResponse> handler;

        public CreateHandlerProxy(IDefaultHandlerFactory factory)
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            handler = (ICreateHandler<TRow, TSaveRequest, TSaveResponse>) factory.CreateHandler<ISaveRequestProcessor>(typeof(TRow));
        }

        public TSaveResponse Create(IUnitOfWork uow, TSaveRequest request)
        {
            return handler.Create(uow, request);
        }
    }

    internal class CreateHandlerProxy<TRow, TSaveRequest>
        : CreateHandlerProxy<TRow, TSaveRequest, SaveResponse>, ICreateHandler<TRow, TSaveRequest>
        where TRow : class, IRow, IIdRow, new()
        where TSaveRequest : SaveRequest<TRow>, new()
    {
        public CreateHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
    }

    internal class CreateHandlerProxy<TRow>
        : CreateHandlerProxy<TRow, SaveRequest<TRow>, SaveResponse>, ICreateHandler<TRow>
        where TRow : class, IRow, IIdRow, new()
    {
        public CreateHandlerProxy(IDefaultHandlerFactory factory) : base(factory) { }
    }
}