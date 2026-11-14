namespace Serenity.Services;

/// <summary>
/// Optional interface of a list response whose total count can be set after the list query is executed, e.g. by a behavior
/// that counted the records in another way, so the list query doesn't count them again. <see cref="ListResponse{T}"/>
/// implements it.
/// </summary>
public interface IListResponseTotalCount
{
    /// <summary>
    /// Sets the total count, and whether there are more records than the returned ones.
    /// </summary>
    /// <param name="totalCount">The total count.</param>
    /// <param name="more">Whether there are more records beyond the returned page.</param>
    void SetTotalCount(int totalCount, bool? more);
}
