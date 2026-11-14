using System.Collections;

namespace Serenity.Services;

/// <summary>
/// The interface for list response. This is used to easily access 
/// list response members as the <see cref="ListResponse{T}"/>
/// class itself is generic.
/// </summary>
public interface IListResponse
{
    /// <summary>
    /// List of entities
    /// </summary>
    IList Entities { get; set; }

    /// <summary>
    /// Total count of the records. This may be different
    /// than the number of returned records when paging is active, 
    /// e.g. when Skip/Take parameters are assigned.
    /// </summary>
    int TotalCount { get; set; }

    /// <summary>
    /// Number of records skipped, passed from the ListRequest
    /// </summary>
    int Skip { get; set; }

    /// <summary>
    /// Number of records taken, passed from the ListRequest
    /// </summary>
    int Take { get; set; }

    /// <summary>
    /// Whether more records are available beyond the returned page, when requested.
    /// </summary>
    bool? More { get; set; }

    /// <summary>
    /// The aggregates of all the rows the request matches, set by a behavior that supports them.
    /// </summary>
    AggregateGroup? Aggregates { get; set; }
}