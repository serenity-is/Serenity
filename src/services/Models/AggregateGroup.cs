namespace Serenity.Services;

/// <summary>
/// The aggregates of a list request for a set of rows: all the rows the request matches at the root,
/// see <see cref="ListResponse{T}.Aggregates"/>, and the rows of a group under it, one level for each
/// <see cref="ListRequest.GroupBy"/>. They are calculated from all the rows, and not from the page
/// that is returned.
/// </summary>
public class AggregateGroup
{
    /// <summary>
    /// The key of <see cref="Values"/> for <see cref="AggregateType.Count"/>.
    /// </summary>
    public const string CountKey = "count";

    /// <summary>
    /// The key of <see cref="Values"/> for <see cref="AggregateType.Sum"/>.
    /// </summary>
    public const string SumKey = "sum";

    /// <summary>
    /// The key of <see cref="Values"/> for <see cref="AggregateType.Avg"/>.
    /// </summary>
    public const string AvgKey = "avg";

    /// <summary>
    /// The key of <see cref="Values"/> for <see cref="AggregateType.Min"/>.
    /// </summary>
    public const string MinKey = "min";

    /// <summary>
    /// The key of <see cref="Values"/> for <see cref="AggregateType.Max"/>.
    /// </summary>
    public const string MaxKey = "max";

    /// <summary>
    /// The key of <see cref="Values"/> for <see cref="AggregateType.DistinctCount"/>.
    /// </summary>
    public const string DistinctCountKey = "distinct";

    /// <summary>
    /// Gets the key of <see cref="Values"/> for an aggregate type, which are the ones of the
    /// aggregators of the grids for the types they have.
    /// </summary>
    /// <param name="type">The aggregate type.</param>
    /// <exception cref="ArgumentOutOfRangeException">The type is not one of the known ones.</exception>
    public static string GetKey(AggregateType type)
    {
        return type switch
        {
            AggregateType.Count => CountKey,
            AggregateType.Sum => SumKey,
            AggregateType.Avg => AvgKey,
            AggregateType.Min => MinKey,
            AggregateType.Max => MaxKey,
            AggregateType.DistinctCount => DistinctCountKey,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    /// <summary>
    /// The value of the group field for the rows of the group, as it is in the entities, so a row
    /// can be matched to its group. It is null at the root, and for the group of the rows whose
    /// field is null.
    /// </summary>
    public object? Key { get; set; }

    /// <summary>
    /// The number of the rows.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// The values of the aggregates, by the key of their type, see <see cref="GetKey"/>, then by
    /// the name of their field, e.g. <c>Values["sum"]["Freight"]</c>, as the totals of a grid are.
    /// An aggregate that is not calculated, e.g. as its field can't be, is left out. The count
    /// without a field is only in <see cref="Count"/>.
    /// </summary>
    public Dictionary<string, Dictionary<string, object?>>? Values { get; set; }

    /// <summary>
    /// The groups of the next level, in their order. It is null for a group of the last level,
    /// and at the root when the rows are not grouped, which is also when they are asked to be,
    /// but can't.
    /// </summary>
    public List<AggregateGroup>? Groups { get; set; }

    /// <summary>
    /// True when there are more groups than the ones in <see cref="Groups"/>, as their number is
    /// limited. The values of this group are still of all of its rows.
    /// </summary>
    public bool? More { get; set; }
}
