using System;
using System.Linq;

namespace Serenity.Services
{
    public static class RequestHandlerExtensions
    {
        public static ListRequest CreateRequest(this IListRequestHandler handler)
        {
            return (ListRequest)Activator.CreateInstance(handler.GetRequestType());
        }

        public static RetrieveRequest CreateRequest(this IRetrieveRequestHandler handler)
        {
            return (RetrieveRequest)Activator.CreateInstance(handler.GetRequestType());
        }

        public static DeleteRequest CreateRequest(this IDeleteRequestHandler handler)
        {
            return (DeleteRequest)Activator.CreateInstance(handler.GetRequestType());
        }

        public static SaveRequest<TRow> CreateRequest<TRow>(this ISaveRequestHandler handler)
        {
            return (SaveRequest<TRow>)Activator.CreateInstance(handler.GetRequestType());
        }

        public static ISaveRequest CreateRequest(this ISaveRequestHandler handler)
        {
            return (ISaveRequest)Activator.CreateInstance(handler.GetRequestType());
        }

        public static Type GetRequestType(this IRequestHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return handler.GetType().GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IRequestType<>))?.GetGenericArguments()[0];
        }

        public static Type GetResponseType(this IRequestHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return handler.GetType().GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IResponseType<>))?.GetGenericArguments()[0];
        }
    }
}