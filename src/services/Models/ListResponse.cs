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
    public List<T> Entities { get; set; } = [];

    /// <summary>
    /// List of distinct values, if DistinctFields are passed in the list request.
    /// Values for each distinct row are appended in the requested field order. When
    /// multiple fields are requested, the list is flat and values are not grouped into tuples.
    /// </summary>
    public List<object?>? Values { get; set; }

    /// <inheritdoc/>
    public int TotalCount { get; set; }

    /// <inheritdoc/>
    public int Skip { get; set; }

    /// <inheritdoc/>
    public int Take { get; set; }
}