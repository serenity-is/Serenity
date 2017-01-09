
namespace Serenity.Services
{
    public interface IRetrieveResponse
    {
        object Entity { get; }
    }

    public class RetrieveResponse<T> : ServiceResponse, IRetrieveResponse
    {
        object IRetrieveResponse.Entity { get { return Entity; } }

        public T Entity { get; set; }
    }
}