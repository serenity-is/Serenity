using System.Threading;
using System.Threading.Tasks;

namespace Serenity.Services
{
    /// <summary>
    /// Abstraction for delete request handlers with a Process method.
    /// </summary>
    [GenericHandlerType(typeof(DeleteRequestHandler<>))]
    public interface IDeleteRequestProcessor : IDeleteRequestHandler
    {
        /// <summary>
        /// Processes the <see cref="DeleteRequest"/> and returns a <see cref="DeleteResponse"/>
        /// </summary>
        /// <param name="uow">Unit of work</param>
        /// <param name="request">Delete request</param>
        DeleteResponse Process(IUnitOfWork uow, DeleteRequest request);

        /// <summary>
        /// Processes the <see cref="DeleteRequest"/> and returns a <see cref="DeleteResponse"/>
        /// </summary>
        /// <param name="uow">Unit of work</param>
        /// <param name="request">Delete request</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        Task<DeleteResponse> ProcessAsync(IUnitOfWork uow, DeleteRequest request, CancellationToken cancellationToken);
    }
}