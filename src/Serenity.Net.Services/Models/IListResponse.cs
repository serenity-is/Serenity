using System.Collections;

namespace Serenity.Services
{
    public interface IListResponse
    {
        IList Entities { get; }
        int TotalCount { get; }
        int Skip { get; }
        int Take { get; }
    }
}