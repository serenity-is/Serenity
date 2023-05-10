using System.Collections;

namespace Serenity.Services;

/// <summary>
/// The response model for a list service.
/// </summary>
/// <typeparam name="T">Type of the returned entities.</typeparam>
public class ListResponse<T> : ServiceResponse, IListResponse
{
    IList IListResponse.Entities => Entities;

    /// <summary>
    /// Entities
    /// </summary>
    public List<T> Entities { get; set; }

    /// <summary>
    /// List of distinct values, if DistinctFields are passed
    /// in the list request. Each element of the list is
    /// an array of distinct values if multiple distinct fields
    /// are requested.
    /// </summary>
    public List<object> Values { get; set; }

    /// <inheritdoc/>
    public int TotalCount { get; set; }

    /// <inheritdoc/>
    public int Skip { get; set; }

    /// <inheritdoc/>
    public int Take { get; set; }
}