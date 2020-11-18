
namespace Serenity.Services
{
    public interface IRequestHandler
    {
    }

    public interface IRequestHandler<TRow> : IRequestHandler
    {
    }

    public interface IRequestHandler<TRow, TRequest, TResponse> : 
        IRequestHandler<TRow>, IRequestType<TRequest>, IResponseType<TResponse>
    {
    }
}